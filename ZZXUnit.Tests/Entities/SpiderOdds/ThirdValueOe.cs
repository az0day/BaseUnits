using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// ThirdValueHdp
    /// </summary>
    [Serializable]
    public class ThirdValueOe : ThirdValueDataBase
    {
        #region .Implements.
        /// <summary>
        /// Category
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.Oe;
            }
        }

        public override long CreateOwnHash()
        {
            return CreateUniqueHashCode(Category, FirstHalf);
        }

        public override bool EqualValues(ThirdValueDataBase value)
        {
            var nv = (ThirdValueOe)value;
            var nvs = string.Format("{0}:{1}", nv.Odd, nv.Even);
            var ovs = string.Format("{0}:{1}", Odd, Even);
            return nvs.Equals(ovs);
        }
        #endregion

        /// <summary>
        /// Odd
        /// </summary>
        public decimal Odd { get; set; }

        /// <summary>
        /// Even
        /// </summary>
        public decimal Even { get; set; }
    }
}
