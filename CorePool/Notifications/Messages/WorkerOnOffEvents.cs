using CorePool.Persistence.Model;

namespace CorePool.Notifications.Messages
{
    public class WorkerOnOffEvents
    {
        public string PoolId { get; set; }
        public double Hashrate { get; set; }
        public string Miner { get; set; }
        public string Worker { get; set; }
    }
}
