using System;
using System.Linq;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    [Serializable]
    public class ThirdValueDataBase
    {
        #region .CONST.
        /// <summary>
        /// ERROR ODDS
        /// </summary>
        private const decimal E_ODDS = -999m;

        /// <summary>
        /// INVALID ODDS
        /// </summary>
        private const decimal INVALID_ODDS = 0m;

        ///// <summary>
        ///// 默认未始化的日期
        ///// </summary>
        //protected static DateTime DefaultDate = new DateTime(1900, 1, 1);
        #endregion

        #region .For Implements.
        /// <summary>
        /// 基类默认未定义类型: Undefined
        /// </summary>
        public virtual ThirdDataCategoryEnum Category
        {
            get
            {
                return ThirdDataCategoryEnum.Undefined;
            }
        }

        /// <summary>
        /// Generate Hash
        /// </summary>
        /// <returns></returns>
        public virtual long CreateOwnHash()
        {
            return Extensions.CreateFastHashCode(Category);
        }

        public virtual bool EqualValues(ThirdValueDataBase value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region .Properties.
        /// <summary>
        /// Own Hash
        /// </summary>
        public long OwnHash { get; set; }

        /// <summary>
        /// 已关闭
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        /// 已移除
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// 全场（默认：True）
        /// </summary>
        public bool FirstHalf { get; set; }
        #endregion

        #region .Methods.
        /// <summary>
        /// Odds Valid
        /// </summary>
        /// <param name="odds"></param>
        /// <returns></returns>
        protected bool OddsValid(params decimal[] odds)
        {
            return !(odds == null || odds.Any(value => value == INVALID_ODDS || value == E_ODDS));
        }

        protected static long CreateUniqueHashCode(params object[] texts)
        {
            return Extensions.CreateFastHashCode(texts);
        }
        #endregion

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
    }
}
