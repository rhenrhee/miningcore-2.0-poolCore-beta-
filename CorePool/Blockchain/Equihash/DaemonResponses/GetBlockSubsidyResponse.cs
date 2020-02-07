using Newtonsoft.Json;

namespace CorePool.Blockchain.Equihash.DaemonResponses
{
    public class ZCashBlockSubsidy
    {
        public decimal Miner { get; set; }
        public decimal? Founders { get; set; }
        public decimal? Community { get; set; }
    }
}
