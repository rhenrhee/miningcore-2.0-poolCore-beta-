using System.Collections.Generic;

namespace CorePool.Blockchain
{
    public static class DevDonation
    {
        public const decimal Percent = 0.1m;

        public static readonly Dictionary<string, string> Addresses = new Dictionary<string, string>
        {
            { "BTC", "18An3WtM3JjA1VYR4rCZFRLfcFafFZx4PY" },
            { "BCH", "qzpcmcsp6xhkyj3dk50hwmge5ulhvms9vqtlf47qz2" },
            { "LTC", "LNKSQZpzvkYQ3mUrWuApgoN8nqhXefrhrg" },
            { "ETH", "0x661652bafec7b288bf00cd403f07ed9532d05d4a" },
            { "ETC", "0x1856f8937c881943ba600b0f82123ff101d9acdc" },
            { "BTG", "GQzademMNF17X7NNPf6RvPCzCYF3R2o1Nq" },
            { "RVN", "RS1VbGPkZwADmQtfvkoG1YkykCXVd5t1K2" },
        };
    }

    public static class CoinMetaData
    {
        public const string BlockHeightPH = "$height$";
        public const string BlockHashPH = "$hash$";
    }
}
