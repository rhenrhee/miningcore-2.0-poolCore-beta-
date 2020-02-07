using Newtonsoft.Json;

namespace CorePool.Blockchain.Cryptonote.StratumRequests
{
    public class CryptonoteGetJobRequest
    {
        [JsonProperty("id")]
        public string WorkerId { get; set; }
    }
}
