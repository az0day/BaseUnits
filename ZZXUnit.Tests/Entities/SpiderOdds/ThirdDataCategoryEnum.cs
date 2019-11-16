using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    [Serializable]
    public enum ThirdDataCategoryEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// 联赛信息
        /// </summary>
        League = 1,

        /// <summary>
        /// 球队信息
        /// </summary>
        Team = 2,

        /// <summary>
        /// 球赛信息
        /// </summary>
        Match = 3,

        /// <summary>
        /// 联赛关联信息
        /// </summary>
        LeagueMapping = 4,

        /// <summary>
        /// 球队关联信息
        /// </summary>
        TeamMapping = 5,




        /// <summary>
        /// Period
        /// </summary>
        Period = 20,

        /// <summary>
        /// 比分信息
        /// </summary>
        Score = 21,

        /// <summary>
        /// 红卡信息
        /// </summary>
        RedCard = 22,

        /// <summary>
        /// 黄卡信息
        /// </summary>
        YellowCard = 23,

        /// <summary>
        /// HDP
        /// </summary>
        Hdp = 24,

        /// <summary>
        /// OU
        /// </summary>
        Ou = 25,

        /// <summary>
        /// OE
        /// </summary>
        Oe = 26,

        /// <summary>
        /// 1X2
        /// </summary>
        X12 = 27,

        /// <summary>
        /// Money Line
        /// </summary>
        MoneyLine = 28,

        /// <summary>
        /// Match Winner
        /// </summary>
        MatchWinner = 29,

        /// <summary>
        /// Game Hdp
        /// </summary>
        GameHdp = 30,
    }
}
