using System;
using System.Security.Cryptography;
using System.Threading;

namespace CorePool.Blockchain.Bitcoin
{
    public class BitcoinExtraNonceProvider : ExtraNonceProviderBase
    {
        public BitcoinExtraNonceProvider() : base(4)
        {
        }
    }
}
