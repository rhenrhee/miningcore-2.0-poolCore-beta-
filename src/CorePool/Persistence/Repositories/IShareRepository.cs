using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CorePool.Persistence.Model;
using CorePool.Persistence.Model.Projections;

namespace CorePool.Persistence.Repositories
{
    public interface IShareRepository
    {
        Task InsertAsync(IDbConnection con, IDbTransaction tx, Share share);
        Task BatchInsertAsync(IDbConnection con, IDbTransaction tx, IEnumerable<Share> shares);
        Task<Share[]> ReadSharesBeforeCreatedAsync(IDbConnection con, string poolId, DateTime before, bool inclusive, int pageSize);
        Task<Share[]> ReadSharesBeforeAndAfterCreatedAsync(IDbConnection con, string poolId, DateTime before, DateTime after, bool inclusive, int pageSize);
        Task<Share[]> PageSharesBetweenCreatedAsync(IDbConnection con, string poolId, DateTime start, DateTime end, int page, int pageSize);

        Task<long> CountSharesBeforeCreatedAsync(IDbConnection con, IDbTransaction tx, string poolId, DateTime before);
        Task DeleteSharesBeforeCreatedAsync(IDbConnection con, IDbTransaction tx, string poolId, DateTime before);

        Task<long> CountSharesBetweenCreatedAsync(IDbConnection con, string poolId, string miner, DateTime? start, DateTime? end);
        Task<double?> GetAccumulatedShareDifficultyBetweenCreatedAsync(IDbConnection con, string poolId, DateTime start, DateTime end);
        Task<MinerWorkerHashes[]> GetAccumulatedShareDifficultyTotalAsync(IDbConnection con, string poolId);
        Task<MinerWorkerHashes[]> GetHashAccumulationBetweenCreatedAsync(IDbConnection con, string poolId, DateTime start, DateTime end);
    }
}
