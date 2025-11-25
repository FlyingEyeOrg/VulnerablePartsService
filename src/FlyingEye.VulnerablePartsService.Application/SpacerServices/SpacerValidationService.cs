using FlyingEye.Exceptions;
using FlyingEye.Spacers;
using FlyingEye.Spacers.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;

namespace FlyingEye.SpacerServices
{
    /// <summary>
    /// 垫片验证服务
    /// </summary>
    public class SpacerValidationService : ApplicationService
    {
        private readonly ISpacerValidationDataRepository _spacerValidationDataRepository;

        public SpacerValidationService(ISpacerValidationDataRepository spacerValidationDataRepository)
        {
            _spacerValidationDataRepository = spacerValidationDataRepository;
        }

        /// <summary>
        /// 获取最新的垫片参数信息（优化版）
        /// </summary>
        public async Task<SpacerValidationDataResult> GetLatestAsync(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new HttpBadRequestException("设备资源号不能为空");
            }

            var trimmedResourceId = resourceId.Trim(); // 提前Trim
            var queryable = await _spacerValidationDataRepository.GetQueryableAsync();

            var latestEntity = await queryable
                .Where(x => x.ResourceId == trimmedResourceId) // 使用已Trim的值
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefaultAsync();

            if (latestEntity == null)
            {
                throw new HttpNotFoundException($"PE 未维护设备 {trimmedResourceId} 的垫片信息");
            }

            return ObjectMapper.Map<SpacerValidationDataModel, SpacerValidationDataResult>(latestEntity);
        }

        /// <summary>
        /// 添加新的垫片参数信息
        /// </summary>
        public async Task AddAsync(SpacerValidationData data)
        {
            // 1. 校验所有必填字段
            VerifyAllFields(data);

            // 2. 执行Trim处理
            var trimmedData = TrimAllFields(data);

            // 3. 创建新的实体
            var entity = new SpacerValidationDataModel(
                site: trimmedData.Site,
                resourceId: trimmedData.ResourceId,
                @operator: trimmedData.Operator,
                modelPn: trimmedData.ModelPn,
                date: trimmedData.Date,
                bigCoatingWidth: trimmedData.BigCoatingWidth,
                smallCoatingWidth: trimmedData.SmallCoatingWidth,
                whiteSpaceWidth: trimmedData.WhiteSpaceWidth,
                aT11Width: trimmedData.AT11Width,
                thickness: trimmedData.Thickness,
                aBSite: trimmedData.ABSite
            );

            // 4. 保存到数据库
            await _spacerValidationDataRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 校验所有字段
        /// </summary>
        private void VerifyAllFields(SpacerValidationData data)
        {
            var errors = new List<string>();

            // 1. 必填字段验证（8个核心参数）
            if (string.IsNullOrWhiteSpace(data.ResourceId))
                errors.Add("设备资源号不能为空");

            if (string.IsNullOrWhiteSpace(data.ModelPn))
                errors.Add("机种不能为空");

            if (string.IsNullOrWhiteSpace(data.Date))
                errors.Add("时间不能为空");

            if (string.IsNullOrWhiteSpace(data.BigCoatingWidth))
                errors.Add("大膜宽不能为空");

            if (string.IsNullOrWhiteSpace(data.SmallCoatingWidth))
                errors.Add("小膜宽不能为空");

            if (string.IsNullOrWhiteSpace(data.WhiteSpaceWidth))
                errors.Add("极耳宽度不能为空");

            if (string.IsNullOrWhiteSpace(data.AT11Width))
                errors.Add("AT11宽度不能为空");

            if (string.IsNullOrWhiteSpace(data.Thickness))
                errors.Add("垫片厚度不能为空");

            if (string.IsNullOrWhiteSpace(data.ABSite))
                errors.Add("A/B面不能为空");

            // 2. 可选字段的基础验证（Site和Operator）
            if (data.Site != null && string.IsNullOrWhiteSpace(data.Site))
                errors.Add("基地编号不能为空");

            if (data.Operator != null && string.IsNullOrWhiteSpace(data.Operator))
                errors.Add("操作员卡号不能为空");

            // 3. 格式验证
            if (!string.IsNullOrWhiteSpace(data.ABSite))
            {
                var absite = data.ABSite.Trim().ToUpper();
                if (absite != "A" && absite != "B")
                {
                    errors.Add("A/B面只能是A或B");
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Site))
            {
                if (data.Site.Trim().Length > 10) // 假设基地编号最大长度10
                {
                    errors.Add("基地编号长度不能超过10个字符");
                }
            }

            if (!string.IsNullOrWhiteSpace(data.ResourceId))
            {
                if (data.ResourceId.Trim().Length > 50) // 假设设备资源号最大长度50
                {
                    errors.Add("设备资源号长度不能超过50个字符");
                }
            }

            // 4. 数值格式验证（如果这些字段应该是数值）
            if (!string.IsNullOrWhiteSpace(data.BigCoatingWidth) && !IsValidNumber(data.BigCoatingWidth))
                errors.Add("大膜宽格式不正确，应为数值");

            if (!string.IsNullOrWhiteSpace(data.SmallCoatingWidth) && !IsValidNumber(data.SmallCoatingWidth))
                errors.Add("小膜宽格式不正确，应为数值");

            if (!string.IsNullOrWhiteSpace(data.WhiteSpaceWidth) && !IsValidNumber(data.WhiteSpaceWidth))
                errors.Add("极耳宽度格式不正确，应为数值");

            if (!string.IsNullOrWhiteSpace(data.AT11Width) && !IsValidNumber(data.AT11Width))
                errors.Add("AT11宽度格式不正确，应为数值");

            if (!string.IsNullOrWhiteSpace(data.Thickness) && !IsValidNumber(data.Thickness))
                errors.Add("垫片厚度格式不正确，应为数值");

            if (errors.Count > 0)
            {
                throw new HttpBadRequestException($"数据验证失败：\n{string.Join("\n", errors)}");
            }
        }

        /// <summary>
        /// 验证是否为有效数值
        /// </summary>
        private bool IsValidNumber(string value)
        {
            return double.TryParse(value, out _);
        }

        /// <summary>
        /// 对所有字段执行Trim处理
        /// </summary>
        private SpacerValidationData TrimAllFields(SpacerValidationData data)
        {
            return new SpacerValidationData(
                site: data.Site?.Trim() ?? string.Empty,
                resourceId: data.ResourceId?.Trim() ?? string.Empty,
                @operator: data.Operator?.Trim() ?? string.Empty,
                modelPn: data.ModelPn?.Trim() ?? string.Empty,
                date: data.Date?.Trim() ?? string.Empty,
                bigCoatingWidth: data.BigCoatingWidth?.Trim() ?? string.Empty,
                smallCoatingWidth: data.SmallCoatingWidth?.Trim() ?? string.Empty,
                whiteSpaceWidth: data.WhiteSpaceWidth?.Trim() ?? string.Empty,
                aT11Width: data.AT11Width?.Trim() ?? string.Empty,
                thickness: data.Thickness?.Trim() ?? string.Empty,
                aBSite: data.ABSite?.Trim() ?? string.Empty
            );
        }

        /// <summary>
        /// 验证易损件垫片参数
        /// </summary>
        /// <param name="data">垫片参数信息</param>
        /// <returns>void</returns>
        /// <exception cref="HttpBadRequestException">验证失败抛出异常</exception>
        public async Task VerifyAsync(SpacerValidationData data)
        {
            // 1. 参数空值检查
            if (string.IsNullOrWhiteSpace(data.ResourceId))
            {
                throw new HttpBadRequestException("设备资源号不能为空");
            }

            // 2. 查询最新的数据库记录
            var trimmedResourceId = data.ResourceId.Trim();

            var model = await this.GetLatestAsync(trimmedResourceId);

            if (model == null)
            {
                throw new HttpNotFoundException($"PE 未维护设备 {trimmedResourceId} 的垫片信息");
            }

            // 3. 校验8个参数是否相等
            var errors = new List<string>();

            // 使用忽略大小写和空格的比较方式
            if (!string.Equals(data.ModelPn?.Trim(), model.ModelPn?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"机种: 已维护值 '{model.ModelPn}' ≠ 输入值 '{data.ModelPn}'");
            }

            if (!string.Equals(data.Date?.Trim(), model.Date?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"时间: 已维护值 '{model.Date}' ≠ 输入值 '{data.Date}'");
            }

            if (!string.Equals(data.BigCoatingWidth?.Trim(), model.BigCoatingWidth?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"大膜宽: 已维护值 '{model.BigCoatingWidth}' ≠ 输入值 '{data.BigCoatingWidth}'");
            }

            if (!string.Equals(data.SmallCoatingWidth?.Trim(), model.SmallCoatingWidth?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"小膜宽: 已维护值 '{model.SmallCoatingWidth}' ≠ 输入值 '{data.SmallCoatingWidth}'");
            }

            if (!string.Equals(data.WhiteSpaceWidth?.Trim(), model.WhiteSpaceWidth?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"极耳宽度: 已维护值 '{model.WhiteSpaceWidth}' ≠ 输入值 '{data.WhiteSpaceWidth}'");
            }

            if (!string.Equals(data.AT11Width?.Trim(), model.AT11Width?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"AT11宽度: 已维护值 '{model.AT11Width}' ≠ 输入值 '{data.AT11Width}'");
            }

            if (!string.Equals(data.Thickness?.Trim(), model.Thickness?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"垫片厚度: 已维护值 '{model.Thickness}' ≠ 输入值 '{data.Thickness}'");
            }

            if (!string.Equals(data.ABSite?.Trim(), model.ABSite?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"A/B面: 已维护值 '{model.ABSite}' ≠ 输入值 '{data.ABSite}'");
            }

            // 4. 如果有不匹配的字段，抛出异常
            if (errors.Count > 0)
            {
                var errorMessage = $"设备 {trimmedResourceId} 的垫片信息校验失败：\n" + string.Join("\n", errors);
                throw new HttpUnprocessableEntityException(errorMessage);
            }
        }

        /// <summary>
        /// 分页获取指定时间段的记录
        /// </summary>
        public async Task<SpacerRecordsQueryResult> GetPagedRecordsByTimeRangeAsync(SpacerRecordsQueryRequest request)
        {
            request.Validate();

            var trimmedResourceId = request.ResourceId.Trim();
            var queryable = await _spacerValidationDataRepository.GetQueryableAsync();

            // 基础查询条件
            var baseQuery = queryable
                .Where(x => x.ResourceId == trimmedResourceId)
                .Where(x => x.CreationTime >= request.StartTime && x.CreationTime <= request.EndTime);

            // 获取总数
            var totalCount = await baseQuery.CountAsync();

            if (totalCount == 0)
            {
                throw new HttpNotFoundException($"在指定时间段内未找到设备 {trimmedResourceId} 的垫片记录");
            }

            // 获取分页数据
            var records = await baseQuery
                .OrderByDescending(x => x.CreationTime)
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToListAsync();

            var data = ObjectMapper.Map<List<SpacerValidationDataModel>, List<SpacerValidationDataResult>>(records);

            return new SpacerRecordsQueryResult(data, totalCount, request.SkipCount, request.MaxResultCount);
        }
    }
}