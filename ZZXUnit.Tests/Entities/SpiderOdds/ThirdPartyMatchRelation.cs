using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// 第三方数据
    /// </summary>
    [Serializable]
    public class ThirdPartyMatchRelation : ThirdValueDataBase
    {
        /// <summary>
        /// Party
        /// </summary>
        public ThirdPartyEnum Party { get; set; }

        /// <summary>
        /// Sport
        /// </summary>
        public ThirdSportEnum Sport { get; set; }

        /// <summary>
        /// Own Hash
        /// </summary>
        public long AutoId { get; set; }

        /// <summary>
        /// Own Hash
        /// </summary>
        public long LeagueAutoId { get; set; }

        /// <summary>
        /// Own Hash
        /// </summary>
        public long HomeAutoId { get; set; }

        /// <summary>
        /// Own Hash
        /// </summary>
        public long AwayAutoId { get; set; }

        /// <summary>
        /// 第三方的球赛标识
        /// </summary>
        public string MatchId { get; set; }

        /// <summary>
        /// 是否为特殊赛事
        /// </summary>
        public bool IsSpecial { get; set; }

        /// <summary>
        /// Neutral
        /// </summary>
        public bool Neutral { get; set; }

        #region .Implements.
        /// <summary>
        /// 所属分类: Match
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.Match;
            }
        }
        #endregion

        #region .可能会变的属性.
        /// <summary>
        /// League
        /// </summary>
        public string League { get; set; }

        /// <summary>
        /// Home Team
        /// </summary>
        public string Home { get; set; }

        /// <summary>
        /// Away Team
        /// </summary>
        public string Away { get; set; }
        
        /// <summary>
        /// Last Timestamp
        /// </summary>
        public int LastTimestamp { get; set; }

        // 只在创建的时候或单独的事件更新
        // only for creation and update events

        /// <summary>
        /// Current Working Date
        /// </summary>
        public int WorkingDate { get; set; }

        /// <summary>
        /// Current Match Date
        /// </summary>
        public DateTime MatchDate { get; set; }

        /// <summary>
        /// Current Running
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Current Match Date
        /// </summary>
        public DateTime PeriodStartTime = new DateTime(1900, 1, 1);

        /// <summary>
        /// Running Minutes
        /// </summary>
        public int RunningMinutes { get; set; }

        /// <summary>
        /// 当前主队进球数
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// 当前客队进球数
        /// </summary>
        public int AwayScore { get; set; }

        /// <summary>
        /// 红卡
        /// </summary>
        public int HomeRedCard { get; set; }

        /// <summary>
        /// 红卡
        /// </summary>
        public int AwayRedCard { get; set; }

        /// <summary>
        /// 黄卡
        /// </summary>
        public int HomeYellowCard { get; set; }

        /// <summary>
        /// 黄卡
        /// </summary>
        public int AwayYellowCard { get; set; }

        // 只在创建的时候或单独的事件更新
        // only for creation and update events
        #endregion
    }
}
