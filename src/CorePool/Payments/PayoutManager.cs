using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Metadata;
using CorePool.Configuration;
using CorePool.Extensions;
using CorePool.Messaging;
using CorePool.Notifications.Messages;
using CorePool.Persistence;
using CorePool.Persistence.Model;
using CorePool.Persistence.Repositories;
using NLog;
using Contract = CorePool.Contracts.Contract;

namespace CorePool.Payments
{
    /// <summary>
    /// Coin agnostic payment processor
    /// </summary>
    public class PayoutManager
    {
        public PayoutManager(IComponentContext ctx,
            IConnectionFactory cf,
            IBlockRepository blockRepo,
            IShareRepository shareRepo,
            IBalanceRepository balanceRepo,
            IMessageBus messageBus)
        {
            Contract.RequiresNonNull(ctx, nameof(ctx));
            Contract.RequiresNonNull(cf, nameof(cf));
            Contract.RequiresNonNull(blockRepo, nameof(blockRepo));
            Contract.RequiresNonNull(shareRepo, nameof(shareRepo));
            Contract.RequiresNonNull(balanceRepo, nameof(balanceRepo));
            Contract.RequiresNonNull(messageBus, nameof(messageBus));

            this.ctx = ctx;
            this.cf = cf;
            this.blockRepo = blockRepo;
            this.shareRepo = shareRepo;
            this.balanceRepo = balanceRepo;
            this.messageBus = messageBus;
        }

        private readonly IBalanceRepository balanceRepo;
        private readonly IBlockRepository blockRepo;
        private readonly IConnectionFactory cf;
        private readonly IComponentContext ctx;
        private readonly IShareRepository shareRepo;
        private readonly IMessageBus messageBus;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private TimeSpan interval;
        private ClusterConfig clusterConfig;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private async Task ProcessPoolsAsync()
        {
            foreach(var pool in clusterConfig.Pools.Where(x => x.Enabled && x.PaymentProcessing.Enabled))
            {
                logger.Info(() => $"Processing payments for pool {pool.Id}");

                try
                {
                    var family = HandleFamilyOverride(pool.Template.Family, pool);

                    // resolve payout handler
                    var handlerImpl = ctx.Resolve<IEnumerable<Meta<Lazy<IPayoutHandler, CoinFamilyAttribute>>>>()
                        .First(x => x.Value.Metadata.SupportedFamilies.Contains(family)).Value;

                    var handler = handlerImpl.Value;
                    await handler.ConfigureAsync(clusterConfig, pool);

                    // resolve payout scheme
                    var scheme = ctx.ResolveKeyed<IPayoutScheme>(pool.PaymentProcessing.PayoutScheme);

                    await UpdatePoolBalancesAsync(pool, handler, scheme);
                    await PayoutPoolBalancesAsync(pool, handler);
                }

                catch(InvalidOperationException ex)
                {
                    logger.Error(ex.InnerException ?? ex, () => $"[{pool.Id}] Payment processing failed");
                }

                catch(AggregateException ex)
                {
                    switch(ex.InnerException)
                    {
                        case HttpRequestException httpEx:
                            logger.Error(() => $"[{pool.Id}] Payment processing failed: {httpEx.Message}");
                            break;

                        default:
                            logger.Error(ex.InnerException, () => $"[{pool.Id}] Payment processing failed");
                            break;
                    }
                }

                catch(Exception ex)
                {
                    logger.Error(ex, () => $"[{pool.Id}] Payment processing failed");
                }
            }
        }

        private static CoinFamily HandleFamilyOverride(CoinFamily family, PoolConfig pool)
        {
            switch(family)
            {
                case CoinFamily.Equihash:
                    var equihashTemplate = pool.Template.As<EquihashCoinTemplate>();

                    if(equihashTemplate.UseBitcoinPayoutHandler)
                        return CoinFamily.Bitcoin;

                    break;
            }

            return family;
        }

        private async Task UpdatePoolBalancesAsync(PoolConfig pool, IPayoutHandler handler, IPayoutScheme scheme)
        {
            // get pending blockRepo for pool
            var pendingBlocks = await cf.Run(con => blockRepo.GetPendingBlocksForPoolAsync(con, pool.Id));

            // classify
            var updatedBlocks = await handler.ClassifyBlocksAsync(pendingBlocks);

            if(updatedBlocks.Any())
            {
                foreach(var block in updatedBlocks.OrderBy(x => x.Created))
                {
                    logger.Info(() => $"Processing payments for pool {pool.Id}, block {block.BlockHeight}");

                    await cf.RunTx(async (con, tx) =>
                    {
                        if(!block.Effort.HasValue)  // fill block effort if empty
                            await CalculateBlockEffortAsync(pool, block, handler);

                        switch(block.Status)
                        {
                            case BlockStatus.Confirmed:
                                // blockchains that do not support block-reward payments via coinbase Tx
                                // must generate balance records for all reward recipients instead
                                var blockReward = await handler.UpdateBlockRewardBalancesAsync(con, tx, block, pool);

                                await scheme.UpdateBalancesAsync(con, tx, pool, handler, block, blockReward);
                                await blockRepo.UpdateBlockAsync(con, tx, block);
                                break;

                            case BlockStatus.Orphaned:
                            case BlockStatus.Pending:
                                await blockRepo.UpdateBlockAsync(con, tx, block);
                                break;
                        }
                    });
                }
            }

            else
                logger.Info(() => $"No updated blocks for pool {pool.Id}");
        }

        private async Task PayoutPoolBalancesAsync(PoolConfig pool, IPayoutHandler handler)
        {
            var poolBalancesOverMinimum = await cf.Run(con =>
                balanceRepo.GetPoolBalancesOverThresholdAsync(con, pool.Id, pool.PaymentProcessing.MinimumPayment));

            if(poolBalancesOverMinimum.Length > 0)
            {
                try
                {
                    await handler.PayoutAsync(poolBalancesOverMinimum);
                }

                catch(Exception ex)
                {
                    await NotifyPayoutFailureAsync(poolBalancesOverMinimum, pool, ex);
                    throw;
                }
            }

            else
                logger.Info(() => $"No balances over configured minimum payout for pool {pool.Id}");
        }

        private Task NotifyPayoutFailureAsync(Balance[] balances, PoolConfig pool, Exception ex)
        {
            messageBus.SendMessage(new PaymentNotification(pool.Id, ex.Message, balances.Sum(x => x.Amount), pool.Template.Symbol));

            return Task.FromResult(true);
        }

        private async Task CalculateBlockEffortAsync(PoolConfig pool, Block block, IPayoutHandler handler)
        {
            // get share date-range
            var from = DateTime.MinValue;
            var to = block.Created;

            // get last block for pool
            var lastBlock = await cf.Run(con => blockRepo.GetBlockBeforeAsync(con, pool.Id, new[]
            {
                BlockStatus.Confirmed,
                BlockStatus.Orphaned,
                BlockStatus.Pending,
            }, block.Created));

            if(lastBlock != null)
                from = lastBlock.Created;

            // get combined diff of all shares for block
            var accumulatedShareDiffForBlock = await cf.Run(con =>
                shareRepo.GetAccumulatedShareDifficultyBetweenCreatedAsync(con, pool.Id, from, to));

            // handler has the final say
            if(accumulatedShareDiffForBlock.HasValue)
                await handler.CalculateBlockEffortAsync(block, accumulatedShareDiffForBlock.Value);
        }

        #region API-Surface

        public void Configure(ClusterConfig clusterConfig)
        {
            this.clusterConfig = clusterConfig;

            interval = TimeSpan.FromSeconds(clusterConfig.PaymentProcessing.Interval > 0 ?
                clusterConfig.PaymentProcessing.Interval : 600);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                logger.Info(() => "Online");

                while(!cts.IsCancellationRequested)
                {
                    try
                    {
                        await ProcessPoolsAsync();
                    }

                    catch(Exception ex)
                    {
                        logger.Error(ex);
                    }

                    await Task.Delay(interval, cts.Token);
                }
            });
        }

        public void Stop()
        {
            logger.Info(() => "Stopping ..");

            cts.Cancel();

            logger.Info(() => "Stopped");
        }

        #endregion // API-Surface
    }
}
