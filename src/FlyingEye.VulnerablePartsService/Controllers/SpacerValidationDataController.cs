using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using FlyingEye.SpacerServices;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace FlyingEye.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/spacer-validation-data")]
    [Produces("application/json")]
    public class SpacerValidationDataController : AbpController
    {
        private readonly SpacerValidationService _spacerValidationService;

        public SpacerValidationDataController(SpacerValidationService spacerValidationService)
        {
            _spacerValidationService = spacerValidationService;
        }

        /// <summary>
        /// 根据ID获取垫片参数信息
        /// </summary>
        /// <param name="id">垫片参数记录的唯一标识符</param>
        /// <returns>垫片参数详细信息</returns>
        [HttpGet("{id:guid}", Name = nameof(GetByIdAsync))]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerValidationDataResult>> GetByIdAsync(
            [Required] Guid id)
        {
            var result = await _spacerValidationService.GetAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 获取设备的垫片参数信息
        /// </summary>
        /// <param name="resourceId">设备资源号</param>
        /// <param name="abSite">A/B面（A 或 B）</param>
        /// <returns>垫片验证数据</returns>
        [HttpGet("devices/{resourceId}/data")]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpacerValidationDataResult>> GetByResourceIdAsync(
            [Required][FromRoute] string resourceId,
            [FromQuery][Required] string abSite)
        {
            var result = await _spacerValidationService.GetAsync(resourceId, abSite);
            return Ok(result);
        }

        /// <summary>
        /// 添加新的垫片参数信息（首次添加）
        /// </summary>
        /// <param name="data">垫片参数数据</param>
        /// <returns>创建的垫片参数信息</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<SpacerValidationDataResult>> InsertAsync(
            [FromBody] SpacerValidationData data)
        {
            var result = await _spacerValidationService.InsertAsync(data);

            return CreatedAtRoute(
                nameof(GetByIdAsync),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// 更新垫片参数信息（更新现有记录）
        /// </summary>
        /// <param name="data">垫片参数数据</param>
        /// <returns>更新后的垫片参数信息</returns>
        [HttpPut]
        [ProducesResponseType(typeof(SpacerValidationDataResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<SpacerValidationDataResult>> UpdateAsync(
            [FromBody] SpacerValidationData data)
        {
            var result = await _spacerValidationService.UpdateAsync(data);
            return Ok(result);
        }

        /// <summary>
        /// 验证垫片参数信息
        /// </summary>
        /// <param name="data">待验证的垫片数据</param>
        /// <returns>验证结果</returns>
        [HttpPost("verify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<ActionResult> VerifyAsync(
            [FromBody] SpacerValidationData data)
        {
            await _spacerValidationService.VerifyAsync(data);
            return Ok(new { message = "垫片参数验证通过" });
        }
    }
}