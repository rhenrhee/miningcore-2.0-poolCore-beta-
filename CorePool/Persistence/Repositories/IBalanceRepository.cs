using System.Data;
using System.Threading.Tasks;
using CorePool.Configuration;
using CorePool.Persistence.Model;

namespace CorePool.Persistence.Repositories
{
    public interface IBalanceRepository
    {
        Task<int> AddAmountAsync(IDbConnection con, IDbTransaction tx, string poolId, string address, decimal amount, string usage);
        Task<decimal> GetBalanceAsync(IDbConnection con, string poolId, string address);
        Task<decimal> GetBalanceAsync(IDbConnection con, IDbTransaction tx, string poolId, string address);

        Task<Balance[]> GetPoolBalancesOverThresholdAsync(IDbConnection con, string poolId, decimal minimum);
    }
}
