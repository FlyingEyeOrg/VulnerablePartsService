using FlyingEye.Exceptions;
using FlyingEye.Spacers;
using FlyingEye.Spacers.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace FlyingEye.SpacerServices
{
    /// <summary>
    /// 垫片验证服务
    /// </summary>
    public class SpacerValidationService : ApplicationService
    {
        private readonly ISpacerValidationDataRepository _spacerValidationDataRepository;

        private readonly ISpacerValidationDataRecordRepository _spacerValidationDataRecordRepository;

        public SpacerValidationService(
            ISpacerValidationDataRepository spacerValidationDataRepository,
            ISpacerValidationDataRecordRepository spacerValidationDataRecordRepository)
        {
            _spacerValidationDataRepository = spacerValidationDataRepository;
            _spacerValidationDataRecordRepository = spacerValidationDataRecordRepository;
        }

        /// <summary>
        /// 根据ID获取垫片参数信息
        /// </summary>
        /// <param name="id">垫片参数记录的唯一标识符</param>
        /// <returns>垫片参数详细信息</returns>
        /// <exception cref="HttpNotFoundException">当指定ID的记录不存在时抛出</exception>
        public async Task<SpacerValidationDataResult> GetAsync(Guid id)
        {
            // 验证ID有效性
            if (id == Guid.Empty)
            {
                throw new HttpBadRequestException("ID不能为空", "INVALID_ID");
            }

            // 查询数据库
            var model = await _spacerValidationDataRepository.FindAsync(id);

            // 检查记录是否存在
            if (model == null)
            {
                throw new HttpNotFoundException(
                    message: $"未找到ID为 {id} 的垫片参数",
                    details: $"请检查ID是否正确，或该记录可能已被删除"
                );
            }

            return ObjectMapper.Map<SpacerValidationDataModel, SpacerValidationDataResult>(model);
        }

        /// <summary>
        /// 根据ID获取垫片参数记录 
        /// </summary>
        /// <param name="id">记录 id</param>
        /// <returns></returns>
        /// <exception cref="HttpBadRequestException">参数验证异常</exception>
        /// <exception cref="HttpNotFoundException">记录不存在异常</exception>
        public async Task<SpacerValidationDataResult> GetRecordAsync(Guid id)
        {
            // 验证ID有效性
            if (id == Guid.Empty)
            {
                throw new HttpBadRequestException("ID不能为空", "INVALID_ID");
            }

            // 查询数据库
            var model = await _spacerValidationDataRecordRepository.FindAsync(id);

            // 检查记录是否存在
            if (model == null)
            {
                throw new HttpNotFoundException(
                    message: $"未找到ID为 {id} 的垫片参数记录",
                    details: $"请检查ID是否正确，或该记录可能已被删除"
                );
            }

            return ObjectMapper.Map<SpacerValidationDataRecordModel, SpacerValidationDataResult>(model);
        }

        /// <summary>
        /// 获取获取指定设备的垫片信息
        /// </summary>
        public async Task<SpacerValidationDataResult> GetAsync(string resourceId, string abSite)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new HttpBadRequestException("设备资源号不能为空");
            }

            if (string.IsNullOrWhiteSpace(abSite))
            {
                throw new HttpBadRequestException("A/B面不能为空");
            }

            var trimmedResourceId = resourceId.Trim();
            var trimmedAbSite = abSite.Trim();
            var queryable = await _spacerValidationDataRepository.GetQueryableAsync();

            if (trimmedAbSite != "A" && trimmedAbSite != "B")
            {
                throw new HttpBadRequestException("A/B面只能是A或B");
            }

            var entity = await queryable
                .Where(x => x.ResourceId == trimmedResourceId && x.ABSite == trimmedAbSite)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new HttpNotFoundException($"PE 未维护设备 {trimmedResourceId} 的 {trimmedAbSite} 面垫片信息");
            }

            return ObjectMapper.Map<SpacerValidationDataModel, SpacerValidationDataResult>(entity);
        }

        /// <summary>
        /// 插入一个垫片参数信息
        /// </summary>
        /// <param name="data">垫片参数</param>
        /// <returns>返回插入的数据</returns>
        public async Task<SpacerValidationDataResult> InsertAsync(SpacerValidationData data)
        {
            // 1. 校验所有必填字段
            VerifyAllFields(data);

            // 2. 执行Trim处理
            var trimmedData = TrimAllFields(data);

            // 3. 检查是否已经存在相同设备+A/B面的记录
            var existingEntity = await _spacerValidationDataRepository.FindAsync(
                item => item.ResourceId == trimmedData.ResourceId
                && item.ABSite == trimmedData.ABSite);

            if (existingEntity != null)
            {
                throw new HttpConflictException(
                    message: $"设备 {trimmedData.ResourceId} 的 {trimmedData.ABSite} 面垫片信息已存在",
                    details: $"请使用更新接口或删除现有记录后重试。现有记录ID: {existingEntity.Id}"
                );
            }

            // 4. 在事务中插入数据到两个表
            using var uow = this.UnitOfWorkManager.Begin();

            // 插入到主表
            var mainEntity = ObjectMapper.Map<SpacerValidationData, SpacerValidationDataModel>(trimmedData);
            mainEntity = await _spacerValidationDataRepository.InsertAsync(mainEntity);

            // 插入到记录表
            var recordEntity = ObjectMapper.Map<SpacerValidationData, SpacerValidationDataRecordModel>(trimmedData);
            await _spacerValidationDataRecordRepository.InsertAsync(recordEntity);

            await uow.CompleteAsync();   // 提交事务

            Logger.LogInformation($"成功插入设备 {trimmedData.ResourceId} 的 {trimmedData.ABSite} 面垫片参数，主表ID: {mainEntity.Id}");

            return ObjectMapper.Map<SpacerValidationDataModel, SpacerValidationDataResult>(mainEntity);
        }

        /// <summary>
        /// 更新垫片参数信息
        /// </summary>
        public async Task<SpacerValidationDataResult> UpdateAsync(SpacerValidationData data)
        {
            // 1. 校验所有必填字段
            VerifyAllFields(data);

            // 2. 执行Trim处理
            var trimmedData = TrimAllFields(data);

            // 3. 检查是否与最新数据完全重复（8个核心参数）
            await CheckForCompleteDuplicateAsync(trimmedData);
            // ↑ 如果记录不存在，这里会抛出 HttpNotFoundException

            // 4. 查询现有记录
            var currentModel = await _spacerValidationDataRepository.FirstAsync(
                item => item.ResourceId == trimmedData.ResourceId
                && item.ABSite == trimmedData.ABSite);

            // 5. 在事务中更新数据
            using var uow = this.UnitOfWorkManager.Begin();

            // 更新主表记录
            ObjectMapper.Map(trimmedData, currentModel);
            currentModel = await _spacerValidationDataRepository.UpdateAsync(currentModel);

            // 创建历史记录
            var recordEntity = ObjectMapper.Map<SpacerValidationData, SpacerValidationDataRecordModel>(trimmedData);
            await _spacerValidationDataRecordRepository.InsertAsync(recordEntity);

            await uow.CompleteAsync();   // 提交事务

            return ObjectMapper.Map<SpacerValidationDataModel, SpacerValidationDataResult>(currentModel);
        }

        /// <summary>
        /// 检查是否与最新数据完全重复（8个核心参数）
        /// </summary>
        private async Task CheckForCompleteDuplicateAsync(SpacerValidationData newData)
        {
            // 获取最新的数据进行比较
            var latestData = await GetAsync(newData.ResourceId, newData.ABSite);

            // 检查8个核心参数是否完全相同
            if (AreCoreParametersIdentical(newData, latestData))
            {
                throw new HttpConflictException(
                    message: "数据重复，8个核心参数与最新记录完全相同",
                    details: GenerateCoreParametersComparison(newData, latestData)
                );
            }
        }

        /// <summary>
        /// 检查8个核心参数是否完全相同（精确匹配，不忽略大小写）
        /// </summary>
        private bool AreCoreParametersIdentical(SpacerValidationData newData, SpacerValidationDataResult latestData)
        {
            // 比较8个核心参数（精确匹配，不忽略大小写）
            return
                string.Equals(newData.ModelPn?.Trim(), latestData.ModelPn?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.Date?.Trim(), latestData.Date?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.BigCoatingWidth?.Trim(), latestData.BigCoatingWidth?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.SmallCoatingWidth?.Trim(), latestData.SmallCoatingWidth?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.WhiteSpaceWidth?.Trim(), latestData.WhiteSpaceWidth?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.AT11Width?.Trim(), latestData.AT11Width?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.Thickness?.Trim(), latestData.Thickness?.Trim(), StringComparison.Ordinal) &&
                string.Equals(newData.ABSite?.Trim(), latestData.ABSite?.Trim(), StringComparison.Ordinal);
        }

        /// <summary>
        /// 生成核心参数对比信息
        /// </summary>
        private string GenerateCoreParametersComparison(SpacerValidationData newData, SpacerValidationDataResult latestData)
        {
            var coreParameters = new List<string>
            {
                $"机种(ModelPn): {newData.ModelPn}",
                $"时间(Date): {newData.Date}",
                $"大膜宽(BigCoatingWidth): {newData.BigCoatingWidth}",
                $"小膜宽(SmallCoatingWidth): {newData.SmallCoatingWidth}",
                $"极耳宽度(WhiteSpaceWidth): {newData.WhiteSpaceWidth}",
                $"AT11宽度(AT11Width): {newData.AT11Width}",
                $"垫片厚度(Thickness): {newData.Thickness}",
                $"A/B面(ABSite): {newData.ABSite}"
            };

            return $"完全相同的8个核心参数: {string.Join("; ", coreParameters)}";
        }

        /// <summary>
        /// 校验所有字段
        /// </summary>
        private void VerifyAllFields(SpacerValidationData data)
        {
            var errors = new List<string>();

            // 1. 必填字段验证
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

            // 2. 可选字段验证
            if (data.Site != null && string.IsNullOrWhiteSpace(data.Site))
                errors.Add("基地编号不能为空");

            if (data.Operator != null && string.IsNullOrWhiteSpace(data.Operator))
                errors.Add("操作员卡号不能为空");

            // 3. 格式验证
            if (!string.IsNullOrWhiteSpace(data.ABSite))
            {
                var absite = data.ABSite.Trim();
                if (absite != "A" && absite != "B")
                {
                    errors.Add("A/B面只能是A或B");
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Site) && data.Site.Trim().Length > 10)
                errors.Add("基地编号长度不能超过10个字符");

            if (!string.IsNullOrWhiteSpace(data.ResourceId) && data.ResourceId.Trim().Length > 50)
                errors.Add("设备资源号长度不能超过50个字符");

            // 4. 数值格式验证
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
                // 优化：简洁的消息 + 详细的错误列表
                throw new HttpBadRequestException(
                    message: "数据验证失败",
                    details: string.Join("; ", errors)
                );
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
        public async Task VerifyAsync(SpacerValidationData data)
        {
            // 1. 参数空值检查
            if (string.IsNullOrWhiteSpace(data.ResourceId))
            {
                throw new HttpBadRequestException("设备资源号不能为空");
            }

            // 2. 查询最新的数据库记录
            var trimmedResourceId = data.ResourceId.Trim();
            var model = await GetAsync(trimmedResourceId, data.ABSite);

            // 3. 校验参数是否匹配
            var errors = new List<string>();

            if (!string.Equals(data.ModelPn?.Trim(), model.ModelPn?.Trim(), StringComparison.Ordinal))
                errors.Add($"机种: 已维护值 '{model.ModelPn}' ≠ 输入值 '{data.ModelPn}'");

            if (!string.Equals(data.Date?.Trim(), model.Date?.Trim(), StringComparison.Ordinal))
                errors.Add($"时间: 已维护值 '{model.Date}' ≠ 输入值 '{data.Date}'");

            if (!string.Equals(data.BigCoatingWidth?.Trim(), model.BigCoatingWidth?.Trim(), StringComparison.Ordinal))
                errors.Add($"大膜宽: 已维护值 '{model.BigCoatingWidth}' ≠ 输入值 '{data.BigCoatingWidth}'");

            if (!string.Equals(data.SmallCoatingWidth?.Trim(), model.SmallCoatingWidth?.Trim(), StringComparison.Ordinal))
                errors.Add($"小膜宽: 已维护值 '{model.SmallCoatingWidth}' ≠ 输入值 '{data.SmallCoatingWidth}'");

            if (!string.Equals(data.WhiteSpaceWidth?.Trim(), model.WhiteSpaceWidth?.Trim(), StringComparison.Ordinal))
                errors.Add($"极耳宽度: 已维护值 '{model.WhiteSpaceWidth}' ≠ 输入值 '{data.WhiteSpaceWidth}'");

            if (!string.Equals(data.AT11Width?.Trim(), model.AT11Width?.Trim(), StringComparison.Ordinal))
                errors.Add($"AT11宽度: 已维护值 '{model.AT11Width}' ≠ 输入值 '{data.AT11Width}'");

            if (!string.Equals(data.Thickness?.Trim(), model.Thickness?.Trim(), StringComparison.Ordinal))
                errors.Add($"垫片厚度: 已维护值 '{model.Thickness}' ≠ 输入值 '{data.Thickness}'");

            if (!string.Equals(data.ABSite?.Trim(), model.ABSite?.Trim(), StringComparison.Ordinal))
                errors.Add($"A/B面: 已维护值 '{model.ABSite}' ≠ 输入值 '{data.ABSite}'");

            // 4. 如果有不匹配的字段，抛出异常
            if (errors.Count > 0)
            {
                throw new HttpUnprocessableEntityException(
                    message: $"设备 {trimmedResourceId} 的 {model.ABSite} 面垫片信息校验失败",
                    details: string.Join("; ", errors)
                );
            }
        }

        /// <summary>
        /// 分页获取指定时间段的记录（支持排序）
        /// </summary>
        public async Task<SpacerRecordsQueryResult> GetPagedRecordsByTimeRangeAsync(SpacerRecordsQueryRequest request)
        {
            request.Validate();

            var trimmedResourceId = request.ResourceId.Trim();
            var queryable = await _spacerValidationDataRecordRepository.GetQueryableAsync();

            // 基础查询条件
            var baseQuery = queryable
                .Where(x => x.ResourceId == trimmedResourceId)
                .Where(x => x.CreationTime >= request.StartTime && x.CreationTime <= request.EndTime);

            // 获取总数
            var totalCount = await baseQuery.CountAsync();

            if (totalCount == 0)
            {
                throw new HttpNotFoundException(
                    message: $"在指定时间段内未找到设备 {trimmedResourceId} 的垫片记录",
                    details: $"时间段: {request.StartTime:yyyy-MM-dd} 至 {request.EndTime:yyyy-MM-dd}"
                );
            }

            // 应用排序规则
            var sortedQuery = ApplySorting(baseQuery, request.Sorting);

            // 获取分页数据
            var records = await sortedQuery
                .Skip(request.SkipCount)
                .Take(request.MaxResultCount)
                .ToListAsync();

            var data = ObjectMapper.Map<List<SpacerValidationDataRecordModel>, List<SpacerValidationDataResult>>(records);

            return new SpacerRecordsQueryResult(data, totalCount, request.SkipCount, request.MaxResultCount);
        }

        /// <summary>
        /// 应用排序规则
        /// </summary>
        private IQueryable<SpacerValidationDataRecordModel> ApplySorting(
            IQueryable<SpacerValidationDataRecordModel> query, string sorting)
        {
            if (string.IsNullOrWhiteSpace(sorting))
            {
                // 默认排序：按创建时间降序
                return query.OrderByDescending(x => x.CreationTime);
            }

            // 解析排序字符串（支持多字段排序，如 "CreationTime DESC, Date ASC"）
            var sortFields = sorting.Split(',', StringSplitOptions.RemoveEmptyEntries);

            IOrderedQueryable<SpacerValidationDataRecordModel>? orderedQuery = null;

            foreach (var sortField in sortFields)
            {
                var parts = sortField.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fieldName = parts[0].Trim();
                var sortDirection = parts.Length > 1 ? parts[1].Trim().ToUpper() : "ASC";

                orderedQuery = ApplySingleSort(orderedQuery ?? query, fieldName, sortDirection);
            }

            return orderedQuery ?? query.OrderByDescending(x => x.CreationTime);
        }

        /// <summary>
        /// 应用单个字段排序
        /// </summary>
        private IOrderedQueryable<SpacerValidationDataRecordModel> ApplySingleSort(
            IQueryable<SpacerValidationDataRecordModel> query, string fieldName, string sortDirection)
        {
            var isAscending = sortDirection == "ASC";

            return fieldName.ToLowerInvariant() switch
            {
                "creationtime" => isAscending
                    ? query.OrderBy(x => x.CreationTime)
                    : query.OrderByDescending(x => x.CreationTime),

                "date" => isAscending
                    ? query.OrderBy(x => x.Date)
                    : query.OrderByDescending(x => x.Date),

                "modelpn" => isAscending
                    ? query.OrderBy(x => x.ModelPn)
                    : query.OrderByDescending(x => x.ModelPn),

                "resourceid" => isAscending
                    ? query.OrderBy(x => x.ResourceId)
                    : query.OrderByDescending(x => x.ResourceId),

                "site" => isAscending
                    ? query.OrderBy(x => x.Site)
                    : query.OrderByDescending(x => x.Site),

                "operator" => isAscending
                    ? query.OrderBy(x => x.Operator)
                    : query.OrderByDescending(x => x.Operator),

                "bigcoatingwidth" => isAscending
                    ? query.OrderBy(x => x.BigCoatingWidth)
                    : query.OrderByDescending(x => x.BigCoatingWidth),

                "smallcoatingwidth" => isAscending
                    ? query.OrderBy(x => x.SmallCoatingWidth)
                    : query.OrderByDescending(x => x.SmallCoatingWidth),

                "whitespacewidth" => isAscending
                    ? query.OrderBy(x => x.WhiteSpaceWidth)
                    : query.OrderByDescending(x => x.WhiteSpaceWidth),

                "at11width" => isAscending
                    ? query.OrderBy(x => x.AT11Width)
                    : query.OrderByDescending(x => x.AT11Width),

                "thickness" => isAscending
                    ? query.OrderBy(x => x.Thickness)
                    : query.OrderByDescending(x => x.Thickness),

                "absite" => isAscending
                    ? query.OrderBy(x => x.ABSite)
                    : query.OrderByDescending(x => x.ABSite),

                _ => throw new HttpBadRequestException(
                    message: $"不支持的排序字段: {fieldName}",
                    details: $"支持的排序字段: CreationTime, Date, ModelPn, ResourceId, Site, Operator, BigCoatingWidth, SmallCoatingWidth, WhiteSpaceWidth, AT11Width, Thickness, ABSite"
                )
            };
        }
    }
}