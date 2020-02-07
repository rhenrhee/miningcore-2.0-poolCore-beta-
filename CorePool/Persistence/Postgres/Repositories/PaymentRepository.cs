using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using CorePool.Extensions;
using CorePool.Persistence.Model;
using CorePool.Persistence.Model.Projections;
using CorePool.Persistence.Repositories;
using NLog;

namespace CorePool.Persistence.Postgres.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public PaymentRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        private readonly IMapper mapper;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public async Task InsertAsync(IDbConnection con, IDbTransaction tx, Payment payment)
        {
            logger.LogInvoke();

            var mapped = mapper.Map<Entities.Payment>(payment);

            const string query = "INSERT INTO payments(poolid, coin, address, amount, transactionconfirmationdata, created) " +
                "VALUES(@poolid, @coin, @address, @amount, @transactionconfirmationdata, @created)";

            await con.ExecuteAsync(query, mapped, tx);
        }

        public async Task<Payment[]> PagePaymentsAsync(IDbConnection con, string poolId, string address, int page, int pageSize)
        {
            logger.LogInvoke(new[] { poolId });

            var query = "SELECT * FROM payments WHERE poolid = @poolid ";

            if(!string.IsNullOrEmpty(address))
                query += " AND address = @address ";

            query += "ORDER BY created DESC OFFSET @offset FETCH NEXT (@pageSize) ROWS ONLY";

            return (await con.QueryAsync<Entities.Payment>(query, new { poolId, address, offset = page * pageSize, pageSize }))
                .Select(mapper.Map<Payment>)
                .ToArray();
        }

        public async Task<BalanceChange[]> PageBalanceChangesAsync(IDbConnection con, string poolId, string address, int page, int pageSize)
        {
            logger.LogInvoke(new[] { poolId });

            const string query = "SELECT * FROM balance_changes WHERE poolid = @poolid " +
                "AND address = @address " +
                "ORDER BY created DESC OFFSET @offset FETCH NEXT (@pageSize) ROWS ONLY";

            return (await con.QueryAsync<Entities.BalanceChange>(query, new { poolId, address, offset = page * pageSize, pageSize }))
                .Select(mapper.Map<BalanceChange>)
                .ToArray();
        }

        public async Task<AmountByDate[]> PageMinerPaymentsByDayAsync(IDbConnection con, string poolId, string address, int page, int pageSize)
        {
            logger.LogInvoke(new[] { poolId });

            const string query = "SELECT SUM(amount) AS amount, date_trunc('day', created) AS date FROM payments WHERE poolid = @poolid " +
                "AND address = @address " +
                "GROUP BY date " +
                "ORDER BY date DESC OFFSET @offset FETCH NEXT (@pageSize) ROWS ONLY";

            return (await con.QueryAsync<AmountByDate>(query, new { poolId, address, offset = page * pageSize, pageSize }))
                .ToArray();
        }
    }
}
