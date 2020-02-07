using System;
using System.Data;
using System.Threading.Tasks;
using CorePool.Persistence.Model;
using CorePool.Persistence.Model.Projections;
using MinerStats = CorePool.Persistence.Model.Projections.MinerStats;

namespace CorePool.Persistence.Repositories
{
    public interface IStatsRepository
    {
        Task InsertPoolStatsAsync(IDbConnection con, IDbTransaction tx, PoolStats stats);
        Task InsertMinerWorkerPerformanceStatsAsync(IDbConnection con, IDbTransaction tx, MinerWorkerPerformanceStats stats);
        Task<PoolStats> GetLastPoolStatsAsync(IDbConnection con, string poolId);
        Task<decimal> GetTotalPoolPaymentsAsync(IDbConnection con, string poolId);
        Task<PoolStats[]> GetPoolPerformanceBetweenAsync(IDbConnection con, string poolId, SampleInterval interval, DateTime start, DateTime end);
        Task<MinerStats> GetMinerStatsAsync(IDbConnection con, IDbTransaction tx, string poolId, string miner);
        Task<MinerWorkerHashrate[]> GetPoolMinerWorkerHashratesAsync(IDbConnection con, string poolId);
        Task<MinerWorkerPerformanceStats[]> PagePoolMinersByHashrateAsync(IDbConnection con, string poolId, DateTime from, int page, int pageSize);
        Task<WorkerPerformanceStatsContainer[]> GetMinerPerformanceBetweenHourlyAsync(IDbConnection con, string poolId, string miner, DateTime start, DateTime end);
        Task<WorkerPerformanceStatsContainer[]> GetMinerPerformanceBetweenDailyAsync(IDbConnection con, string poolId, string miner, DateTime start, DateTime end);
        Task<int> DeletePoolStatsBeforeAsync(IDbConnection con, DateTime date);
        Task<int> DeleteMinerStatsBeforeAsync(IDbConnection con, DateTime date);
    }
}
