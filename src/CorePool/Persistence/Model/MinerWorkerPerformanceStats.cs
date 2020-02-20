using System;

namespace CorePool.Persistence.Model
{
    public class MinerWorkerPerformanceStats
    {
        public string PoolId { get; set; }
        //Miner содержит адрес кошелька
        public string Miner { get; set; }
        public string Worker { get; set; } 
        public double Hashrate { get; set; }
        public double SharesPerSecond { get; set; }
        public DateTime Created { get; set; }
    }
}
