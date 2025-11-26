using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using FlyingEye.SpacerServices;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace FlyingEye.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/spacer-validation-data-records")]
    [Produces("application/json")]
    public class SpacerValidationDataRecordController : AbpController
    {
        private readonly SpacerValidationService _spacerValidationService;

        public SpacerValidationDataRecordController(SpacerValidationService spacerValidationService)
        {
            _spacerValidationService = spacerValidationService;
        }

        /// <summary>
        /// 根据ID获取垫片参数记录
        /// </summary>
        /// <param name="id">记录的唯一标识符</param>
        /// <returns>垫片参数记录详细信息</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerValidationDataResult>> GetRecordAsync(
            [Required] Guid id)
        {
            var result = await _spacerValidationService.GetRecordAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 分页查询指定设备的垫片参数记录
        /// </summary>
        /// <param name="resourceId">设备资源号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="sorting">排序字段（如：CreationTime DESC, Date ASC）</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="maxResultCount">每页记录数</param>
        /// <returns>分页的垫片参数记录</returns>
        [HttpGet]
        [Route("devices/{resourceId}/records")]
        [ProducesResponseType(typeof(SpacerRecordsQueryResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerRecordsQueryResult>> GetPagedRecordsAsync(
            [Required][FromRoute] string resourceId,
            [FromQuery][Required] DateTime startTime,
            [FromQuery][Required] DateTime endTime,
            [FromQuery] string sorting = "CreationTime DESC",
            [FromQuery] int skipCount = 0,
            [FromQuery][Range(1, 1000)] int maxResultCount = 20)
        {
            var request = new SpacerRecordsQueryRequest(
                resourceId: resourceId,
                startTime: startTime,
                endTime: endTime,
                sorting: sorting,
                skipCount: skipCount,
                maxResultCount: maxResultCount);

            var result = await _spacerValidationService.GetPagedRecordsByTimeRangeAsync(request);
            return Ok(result);
        }

        // 注意：故意不提供以下方法，因为记录表是自动维护的：
        // - POST (创建) - 记录通过业务操作自动创建
        // - PUT/PATCH (更新) - 记录是只读的历史数据  
        // - DELETE (删除) - 历史记录不允许删除
    }
}