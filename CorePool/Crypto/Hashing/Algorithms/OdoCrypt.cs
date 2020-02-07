using System;
using System.Runtime.CompilerServices;
using CorePool.Contracts;
using CorePool.Extensions;
using CorePool.Native;
using static CorePool.Configuration.BitcoinTemplate;

namespace CorePool.Crypto.Hashing.Algorithms
{
    public class OdoCryptConfig
    {
        public uint OdoCryptShapeChangeInterval { get; set; }
    };

    public unsafe class OdoCrypt : IHashAlgorithm
    {
        private static ConditionalWeakTable<BitcoinNetworkParams, OdoCryptConfig> configs = new ConditionalWeakTable<BitcoinNetworkParams, OdoCryptConfig>();

        public void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra)
        {
            Contract.Requires<ArgumentException>(extra.Length >= 4, $"{nameof(extra)} four extra parameters expected");
            Contract.Requires<ArgumentException>(data.Length <= 80, $"{nameof(data)} must be less or equal 80 bytes");
            Contract.Requires<ArgumentException>(result.Length >= 32, $"{nameof(result)} must be greater or equal 32 bytes");

            // derive key parameter
            var nTime = (uint) (ulong) extra[0];
            var networkParams = (BitcoinNetworkParams) extra[3];
            var config = configs.GetValue(networkParams, (bnp) => bnp.Extra.SafeExtensionDataAs<OdoCryptConfig>());
            var key = nTime - nTime % config.OdoCryptShapeChangeInterval;

            fixed (byte* input = data)
            {
                fixed (byte* output = result)
                {
                    LibMultihash.odocrypt(input, output, (uint) data.Length, key);
                }
            }
        }
    }
}
