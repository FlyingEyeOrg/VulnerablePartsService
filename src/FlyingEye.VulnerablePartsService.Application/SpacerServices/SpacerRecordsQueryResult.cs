namespace FlyingEye.SpacerServices
{
    /// <summary>
    /// 分页查询响应结果
    /// </summary>
    public class SpacerRecordsQueryResult
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<SpacerValidationData> Data { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage => SkipCount / MaxResultCount + 1;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)MaxResultCount);

        /// <summary>
        /// 跳过数量
        /// </summary>
        public int SkipCount { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int MaxResultCount { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        public SpacerRecordsQueryResult(
            List<SpacerValidationData> data,
            int totalCount,
            int skipCount,
            int maxResultCount)
        {
            Data = data;
            TotalCount = totalCount;
            SkipCount = skipCount;
            MaxResultCount = maxResultCount;
        }
    }
}
