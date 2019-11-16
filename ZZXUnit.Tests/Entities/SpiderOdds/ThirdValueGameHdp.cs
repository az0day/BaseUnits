using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    /// <summary>
    /// ThirdValueHdp
    /// </summary>
    [Serializable]
    public class ThirdValueGameHdp : ThirdValueDataBase
    {
        #region .Implements.
        /// <summary>
        /// Category: GameHdp
        /// </summary>
        public sealed override ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.GameHdp;
            }
        }

        /// <summary>
        /// 生成唯一标识: (Category, FirstHalf, Hdp)
        /// </summary>
        /// <returns></returns>
        public override long CreateOwnHash()
        {
            return CreateUniqueHashCode(Category, FirstHalf, Hdp);
        }

        public override bool EqualValues(ThirdValueDataBase value)
        {
            var nv = (ThirdValueGameHdp)value;
            var nvs = string.Format("{0}:{1}:{2}", nv.Home, nv.Away, nv.Order);
            var ovs = string.Format("{0}:{1}:{2}", Home, Away, Order);
            return nvs.Equals(ovs);
        }
        #endregion

        /// <summary>
        /// 0: none, 1: home, 2: away
        /// </summary>
        public ThirdFavoriteEnum Favorite
        {
            get
            {
                if (Hdp == 0)
                {
                    return ThirdFavoriteEnum.None;
                }

                return Hdp < 0 ? ThirdFavoriteEnum.Home : ThirdFavoriteEnum.Away;
            }
        }

        /// <summary>
        /// Hdp
        /// </summary>
        public decimal Hdp { get; set; }

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
