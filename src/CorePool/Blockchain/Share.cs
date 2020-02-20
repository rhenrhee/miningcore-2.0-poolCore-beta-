using System;
using ProtoBuf;

namespace CorePool.Blockchain
{
    [ProtoContract]
    public class Share
    {
        /// <summary>
        /// Это, судя по всему, идентификатор пула (btc1 и т.п. из конфигурационного файла)
        /// </summary>
        [ProtoMember(1)]
        public string PoolId { get; set; }

        /// <summary>
        /// Who mined it (wallet address) Определяем, кто именно майнил, но почему-то только по кошельку. Неудобно :( Нужно переделать!
        /// </summary>
        [ProtoMember(2)]
        public string Miner { get; set; }

        /// <summary>
        /// Who mined it (Кто майнил это) 
        /// </summary>
        [ProtoMember(3)]
        public string Worker { get; set; }

        /// <summary>
        /// Mining Software (Чем майнил (ПО))
        /// </summary>
        [ProtoMember(5)]
        public string UserAgent { get; set; }

        /// <summary>
        /// From where was it submitted
        /// </summary>
        [ProtoMember(6)]
        public string IpAddress { get; set; }

        /// <summary>
        /// Submission source (pool, external stratum etc)
        /// </summary>
        [ProtoMember(7)]
        public string Source { get; set; }

        /// <summary>
        /// Stratum difficulty assigned to the miner at the time the share was submitted/accepted (used for payout
        /// calculations)
        /// </summary>
        [ProtoMember(8)]
        public double Difficulty { get; set; }

        /// <summary>
        /// Block this share refers to
        /// </summary>
        [ProtoMember(9)]
        public long BlockHeight { get; set; }

        /// <summary>
        /// Block reward after deducting pool fee and donations
        /// </summary>
        public decimal BlockReward { get; set; }

        /// <summary>
        /// Block reward after deducting pool fee and donations
        /// </summary>
        [ProtoMember(10)]
        public double BlockRewardDouble { get; set; }

        /// <summary>
        /// Block hash
        /// </summary>
        [ProtoMember(11)]
        public string BlockHash { get; set; }

        /// <summary>
        /// If this share presumably resulted in a block
        /// </summary>
        [ProtoMember(12)]
        public bool IsBlockCandidate { get; set; }

        /// <summary>
        /// Arbitrary data to be interpreted by the payment processor specialized
        /// in this coin to verify this block candidate was accepted by the network
        /// </summary>
        [ProtoMember(13)]
        public string TransactionConfirmationData { get; set; }

        /// <summary>
        /// Network difficulty at the time the share was submitted (used for some payout schemes like PPLNS)
        /// </summary>
        [ProtoMember(14)]
        public double NetworkDifficulty { get; set; }

        /// <summary>
        /// When the share was found
        /// </summary>
        [ProtoMember(15)]
        public DateTime Created { get; set; }
    }
}
