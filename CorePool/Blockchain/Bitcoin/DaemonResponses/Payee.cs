using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorePool.Blockchain.Bitcoin.DaemonResponses
{
    public class PayeeBlockTemplateExtra
    {
        public string Payee { get; set; }

        [JsonProperty("payee_amount")]
        public long? PayeeAmount { get; set; }
    }
}
