using System.Data;
using System.Threading.Tasks;

namespace CorePool.Persistence
{
    public interface IConnectionFactory
    {
        Task<IDbConnection> OpenConnectionAsync();
    }
}
