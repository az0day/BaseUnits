using System;
using System.Collections.Generic;

namespace BaseUnits.Core.Entities
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PagedItems<T>
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo Page { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public IList<T> Items { get; set; }
    }
}
