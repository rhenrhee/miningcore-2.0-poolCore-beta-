using System;
using System.Threading;
using System.Threading.Tasks;
using CorePool.Blockchain;
using CorePool.Configuration;
using CorePool.Stratum;

namespace CorePool.Mining
{
    public struct ClientShare
    {
        public ClientShare(StratumClient client, Share share)
        {
            Client = client;
            Share = share;
        }

        public StratumClient Client;
        public Share Share;
    }

    public interface IMiningPool
    {
        PoolConfig Config { get; }
        PoolStats PoolStats { get; }
        BlockchainStats NetworkStats { get; }
        void Configure(PoolConfig poolConfig, ClusterConfig clusterConfig);
        double HashrateFromShares(double shares, double interval);
        Task StartAsync(CancellationToken ctsToken);
        void Stop();
    }
}
