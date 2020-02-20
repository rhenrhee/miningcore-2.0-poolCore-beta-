using System.Data;
using System.Threading.Tasks;
using CorePool.Persistence.Model;
using CorePool.Persistence.Model.Projections;

namespace CorePool.Persistence.Repositories
{
    public interface IPaymentRepository
    {
        Task InsertAsync(IDbConnection con, IDbTransaction tx, Payment payment);

        Task<Payment[]> PagePaymentsAsync(IDbConnection con, string poolId, string address, int page, int pageSize);
        Task<BalanceChange[]> PageBalanceChangesAsync(IDbConnection con, string poolId, string address, int page, int pageSize);
        Task<AmountByDate[]> PageMinerPaymentsByDayAsync(IDbConnection con, string poolId, string address, int page, int pageSize);
    }
}
