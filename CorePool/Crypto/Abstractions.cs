using System;

namespace CorePool.Crypto
{
    public interface IHashAlgorithm
    {
        void Digest(ReadOnlySpan<byte> data, Span<byte> result, params object[] extra);
    }
}
