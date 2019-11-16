using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// ThirdValueHdp
    /// </summary>
    [Serializable]
    public class ThirdValueMoneyLine : ThirdValueDataBase
    {
        #region .Implements.
        /// <summary>
        /// Category
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.MoneyLine;
            }
        }

        public override long CreateOwnHash()
        {
            return CreateUniqueHashCode(Category, FirstHalf);
        }

        public override bool EqualValues(ThirdValueDataBase value)
        {
            var nv = (ThirdValueMoneyLine)value;
            var nvs = string.Format("{0}:{1}", nv.Home, nv.Away);
            var ovs = string.Format("{0}:{1}", Home, Away);
            return nvs.Equals(ovs);
        }
        #endregion

        /// <summary>
        /// Home
        /// </summary>
        public decimal Home { get; set; }

        /// <summary>
        /// Away
        /// </summary>
        public decimal Away { get; set; }
    }
}
