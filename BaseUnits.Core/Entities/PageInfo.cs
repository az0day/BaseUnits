using System;

namespace BaseUnits.Core.Entities
{
    /// <summary>
    /// 分页信息
    /// </summary>
    [Serializable]
    public class PageInfo
    {
        public int PageNow { get; set; }

        public int PageMax { get; set; }

        public int Order { get; set; }

        public int PageSize { get; set; }

        public int RecordCount { get; set; }

        public static PageInfo Calculate(int count, int pageNow, int pageSize, int order = 0)
        {
            // 最大页数
            var pageMax = (int)Math.Ceiling(count / ((decimal)pageSize));

            // fix
            pageNow = Math.Min(pageNow, pageMax);
            pageNow = Math.Max(pageNow, 1);

            return new PageInfo
            {
                PageSize = pageSize,
                Order = order,
                PageMax = pageMax,
                RecordCount = count,
                PageNow = pageNow,
            };
        }
    }
}
