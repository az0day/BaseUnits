using System;
using System.Collections.Generic;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// Third Party Match And Price
    /// </summary>
    [Serializable]
    public class ThirdPartyMatchPrice
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime PostTime { get; set; }

        /// <summary>
        /// 球赛信息
        /// </summary>
        public ThirdPartyMatchRelation Match { get; private set; }

        /// <summary>
        /// 当前所有赔率信息
        /// </summary>
        public Dictionary<long, ThirdValueDataBase> Prices { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="match"></param>
        /// <param name="prices"></param>
        public ThirdPartyMatchPrice(ThirdPartyMatchRelation match, Dictionary<long, ThirdValueDataBase> prices)
        {
            Match = match;
            Prices = prices;
        }
    }
}
