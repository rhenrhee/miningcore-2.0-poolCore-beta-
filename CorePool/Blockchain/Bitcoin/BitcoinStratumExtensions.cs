namespace CorePool.Blockchain.Bitcoin
{
    public class BitcoinStratumExtensions
    {
        public const string VersionRolling = "version-rolling";
        public const string MinimumDiff = "minimum-difficulty";
        public const string SubscribeExtranonce = "subscribe-extranonce";

        public const string VersionRollingMask = VersionRolling + "." + "mask";
        public const string VersionRollingBits = VersionRolling + "." + "min-bit-count";

        public const string MinimumDiffValue = MinimumDiff + "." + "value";
    }
}
