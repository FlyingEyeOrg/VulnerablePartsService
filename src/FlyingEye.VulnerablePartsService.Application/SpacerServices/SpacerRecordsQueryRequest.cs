using FlyingEye.Exceptions;

namespace FlyingEye.SpacerServices
{
    /// <summary>
    /// 分页查询请求参数
    /// </summary>
    public class SpacerRecordsQueryRequest
    {
        public SpacerRecordsQueryRequest(
            string resourceId,
            DateTime startTime,
            DateTime endTime,
            string sorting,
            int skipCount,
            int maxResultCount)
        {
            ResourceId = resourceId;
            StartTime = startTime;
            EndTime = endTime;
            Sorting = sorting;
            SkipCount = skipCount;
            MaxResultCount = maxResultCount;
        }

        /// <summary>
        /// 设备资源号
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 排序规则
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// 跳过数量（用于分页）
        /// </summary>
        public int SkipCount { get; set; } = 0;

        /// <summary>
        /// 每页最大记录数
        /// </summary>
        public int MaxResultCount { get; set; } = 10;

        /// <summary>
        /// 参数验证
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ResourceId))
            {
                throw new HttpBadRequestException("设备资源号不能为空");
            }

            if (StartTime > EndTime)
            {
                throw new HttpBadRequestException("开始时间不能大于结束时间");
            }

            if (SkipCount < 0)
            {
                throw new HttpBadRequestException("跳过数量不能为负数");
            }

            if (MaxResultCount <= 0 || MaxResultCount > 1000)
            {
                throw new HttpBadRequestException("每页记录数应在1-1000之间");
            }
        }
    }
}
