using System;

namespace CorePool.Crypto.Hashing.Algorithms
{
    public class Null : IHashAlgorithm
    {
        public void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra)
        {
            throw new InvalidOperationException("Don't call me");
        }
    }
}
