using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using FlyingEye.Exceptions;
using FlyingEye.SpacerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUglify;
using Volo.Abp.AspNetCore.Mvc;

namespace FlyingEye.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/spacer-validations")]
    [Produces("application/json")]
    public class SpacerValidationController : AbpController
    {
        private readonly SpacerValidationService _spacerValidationService;

        public SpacerValidationController(SpacerValidationService spacerValidationService)
        {
            _spacerValidationService = spacerValidationService;
        }

        [HttpGet]
        [Route("", Name = "GetAsync")]
        public async Task<ActionResult<SpacerValidationDataResult>> GetAsync(Guid id)
        {
            var result = await _spacerValidationService.GetAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 获取设备最新的垫片参数信息
        /// </summary>
        /// <param name="resourceId">设备资源号</param>
        /// <returns>垫片验证数据</returns>
        [HttpGet]
        [Route("devices/{resourceId}/latest")]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerValidationDataResult>> GetLatestByResourceIdAsync(
            [Required][FromRoute] string resourceId)
        {
            var result = await _spacerValidationService.GetLatestAsync(resourceId);
            return Ok(result);
        }

        /// <summary>
        /// 添加新的垫片参数信息
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<ActionResult> AddAsync(SpacerValidationData data)
        {
            var result = await _spacerValidationService.AddAsync(data);

            return CreatedAtRoute(
            "GetAsync",
             new { id = result.Id },  
             new
             {
                 id = result.Id,
                 message = "垫片参数添加成功",
                 resourceId = data.ResourceId
             });
        }

        /// <summary>
        /// 验证垫片参数信息
        /// </summary>
        /// <param name="data">待验证的垫片数据</param>
        /// <returns>验证结果</returns>
        [HttpPost]
        [Route("verify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<ActionResult> VerifyAsync([FromBody] SpacerValidationData data)
        {
            await _spacerValidationService.VerifyAsync(data);
            return Ok(new { message = "垫片参数验证通过" });
        }

        /// <summary>
        /// 分页查询指定时间段的垫片记录
        /// </summary>
        /// <param name="resourceId">设备资源号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="skipCount">跳过数量</param>
        /// <param name="maxResultCount">每页大小</param>
        /// <returns>分页查询结果</returns>
        [HttpGet]
        [Route("devices/{resourceId}/records")]
        [ProducesResponseType(typeof(SpacerRecordsQueryResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerRecordsQueryResult>> GetPagedRecordsByTimeRangeAsync(
            [Required][FromRoute] string resourceId,
            [FromQuery][Required] DateTime startTime,
            [FromQuery][Required] DateTime endTime,
            [FromQuery] int skipCount = 0,
            [FromQuery] int maxResultCount = 10)
        {
            var request = new SpacerRecordsQueryRequest(
                resourceId: resourceId,
                startTime: startTime,
                endTime: endTime,
                skipCount: skipCount,
                maxResultCount: maxResultCount);

            var result = await _spacerValidationService.GetPagedRecordsByTimeRangeAsync(request);
            return Ok(result);
        }
    }
}