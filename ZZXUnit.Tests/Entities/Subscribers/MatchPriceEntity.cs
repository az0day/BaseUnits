using System;
using System.Collections.Generic;
using ZZXUnit.Tests.Entities.SpiderOdds;

namespace ZZXUnit.Tests.Entities.Subscribers
{
    [Serializable]
    public class MatchPriceEntity
    {
        /// <summary>
        /// Client Time
        /// </summary>
        public DateTime PostTime { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Current Time
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Match Info
        /// </summary>
        public ThirdPartyMatchRelation Match { get; set; }

        /// <summary>
        /// Prices
        /// </summary>
        public Dictionary<long, ThirdValueDataBase> Prices { get; set; }
    }
}
