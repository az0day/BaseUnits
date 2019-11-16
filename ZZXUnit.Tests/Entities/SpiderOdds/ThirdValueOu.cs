using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// ThirdValueHdp
    /// </summary>
    [Serializable]
    public class ThirdValueOu : ThirdValueDataBase
    {
        #region .Implements.
        /// <summary>
        /// Category
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.Ou;
            }
        }

        public override long CreateOwnHash()
        {
            return CreateUniqueHashCode(Category, FirstHalf, Ou);
        }

        public override bool EqualValues(ThirdValueDataBase value)
        {
            var nv = (ThirdValueOu)value;
            var nvs = string.Format("{0}:{1}:{2}", nv.Over, nv.Under, nv.Order);
            var ovs = string.Format("{0}:{1}:{2}", Over, Under, Order);
            return nvs.Equals(ovs);
        }
        #endregion

        /// <summary>
        /// Hdp
        /// </summary>
        public decimal Ou { get; set; }

        /// <summary>
        /// Home
        /// </summary>
        public decimal Over { get; set; }

        /// <summary>
        /// Away
        /// </summary>
        public decimal Under { get; set; }
    }
}
