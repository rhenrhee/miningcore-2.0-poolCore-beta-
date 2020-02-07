using System;
using System.Linq;
using System.Threading;
using CorePool.Extensions;

namespace CorePool.Blockchain.Equihash
{
    public class EquihashExtraNonceProvider : ExtraNonceProviderBase
    {
        public EquihashExtraNonceProvider() : base(3)
        {
        }
    }
}
