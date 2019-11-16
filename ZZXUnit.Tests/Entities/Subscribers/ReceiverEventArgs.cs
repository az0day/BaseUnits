using System;
using System.Collections.Generic;
using ZZXUnit.Tests.Entities.SpiderOdds;

namespace ZZXUnit.Tests.Entities.Subscribers
{
    /// <summary>
    /// Receiver EventArgs
    /// </summary>
    [Serializable]
    public class ReceiverEventArgs : EventArgs
    {
        /// <summary>
        /// Timestamp
        /// </summary>
        public int Timestamp { get; private set; }

        /// <summary>
        /// is full data
        /// </summary>
        public bool All { get; private set; }

        /// <summary>
        /// Updates
        /// </summary>
        public IList<MatchPriceEntity> Updates { get; private set; }

        /// <summary>
        /// Removes
        /// </summary>
        public IList<long> Removes { get; private set; }

        /// <summary>
        /// Third Sport
        /// </summary>
        public ThirdSportEnum Sport { get; set; }

        /// <summary>
        /// Third Party
        /// </summary>
        public ThirdPartyEnum Party { get; set; }

        /// <summary>
        /// Parsed Today Time
        /// </summary>
        public DateTime TodayTime { get; set; }

        /// <summary>
        /// Parsed Early Time
        /// </summary>
        public DateTime EarlyTime { get; set; }

        /// <summary>
        /// Parsed Running Time
        /// </summary>
        public DateTime RunningTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="updates"></param>
        /// <param name="removes"></param>
        /// <param name="all"></param>
        public ReceiverEventArgs(int timestamp, IList<MatchPriceEntity> updates, IList<long> removes, bool all = false)
        {
            var defaultTime = new DateTime(1900, 1, 1);

            RunningTime = defaultTime;
            TodayTime = defaultTime;
            EarlyTime = defaultTime;

            Timestamp = timestamp;
            Updates = updates;
            Removes = removes;
            All = all;
        }
    }
}
