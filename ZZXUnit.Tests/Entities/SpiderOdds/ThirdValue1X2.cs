using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// Third 1X2 Odds Value
    /// </summary>
    [Serializable]
    public class ThirdValue1X2 : ThirdValueDataBase
    {
        #region .Implements.
        /// <summary>
        /// Category: X12
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.X12;
            }
        }

        /// <summary>
        /// 生成唯一标识: (Category, FirstHalf)
        /// </summary>
        /// <returns></returns>
        public override long CreateOwnHash()
        {
            return CreateUniqueHashCode(Category, FirstHalf);
        }

        /// <summary>
        /// 是否变化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool EqualValues(ThirdValueDataBase value)
        {
            var nv = (ThirdValue1X2)value;
            var nvs = string.Format("{0}:{1}:{2}", nv.Home, nv.Away, nv.Draw);
            var ovs = string.Format("{0}:{1}:{2}", Home, Away, Draw);
            return nvs.Equals(ovs);
        }
        #endregion

        /// <summary>
        /// Draw
        /// </summary>
        public decimal Draw { get; set; }

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
