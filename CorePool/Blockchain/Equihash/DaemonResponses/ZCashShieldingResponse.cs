using CorePool.JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CorePool.Blockchain.Equihash.DaemonResponses
{
    public class ZCashShieldingResponse
    {
        [JsonProperty("opid")]
        public string OperationId { get; set; }
    }
}
