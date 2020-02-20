using System;
using System.Security.Cryptography;
using CorePool.Contracts;
using CorePool.Native;

namespace CorePool.Crypto.Hashing.Algorithms
{
    /// <summary>
    /// Sha3-256
    /// </summary>
    public unsafe class Sha3_256 : IHashAlgorithm
    {
        public void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra)
        {
            Contract.Requires<ArgumentException>(result.Length >= 32, $"{nameof(result)} must be greater or equal 32 bytes");

            fixed (byte* input = data)
            {
                fixed (byte* output = result)
                {
                    LibMultihash.sha3_256(input, output, (uint) data.Length);
                }
            }
        }
    }
}
