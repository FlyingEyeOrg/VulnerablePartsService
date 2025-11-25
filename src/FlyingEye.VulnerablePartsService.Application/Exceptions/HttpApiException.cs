using System.Net;
using Volo.Abp;

namespace FlyingEye.Exceptions
{
    /// <summary>
    /// 表示 HTTP API 调用过程中发生的异常，包含 HTTP 状态码信息
    /// </summary>
    /// <remarks>
    /// 此异常用于在 API 层统一处理业务逻辑错误，确保返回正确的 HTTP 状态码和错误信息
    /// </remarks>
    public class HttpApiException : UserFriendlyException
    {
        /// <summary>
        /// HTTP 状态码
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// 初始化 HttpApiException 类的新实例
        /// </summary>
        /// <param name="message">面向用户的错误消息</param>
        /// <param name="httpStatusCode">HTTP 状态码</param>
        /// <param name="details">详细的错误信息</param>
        public HttpApiException(string message, HttpStatusCode httpStatusCode, string? details = null)
            : base(message, HttpErrorCodes.GetDefaultErrorCode(httpStatusCode), details)
            => HttpStatusCode = httpStatusCode;

        /// <summary>
        /// 初始化 HttpApiException 类的新实例（包含内部异常）
        /// </summary>
        /// <param name="message">面向用户的错误消息</param>
        /// <param name="innerException">导致当前异常的异常</param>
        /// <param name="httpStatusCode">HTTP 状态码</param>
        /// <param name="details">详细的错误信息</param>
        public HttpApiException(string message, Exception innerException, HttpStatusCode httpStatusCode, string? details = null)
            : base(message: message, code: HttpErrorCodes.GetDefaultErrorCode(httpStatusCode), details: details, innerException: innerException)
            => HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// 表示客户端请求错误（HTTP 400）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 请求参数缺失或格式不正确
    /// 2. 数据验证失败
    /// 3. 必填字段为空
    /// 4. 参数类型不匹配
    /// 示例：
    /// <code>
    /// throw new HttpBadRequestException("邮箱地址不能为空");
    /// throw new HttpBadRequestException("年龄必须是正整数");
    /// </code>
    /// </remarks>
    public class HttpBadRequestException : HttpApiException
    {
        public HttpBadRequestException(string message, string? details = null)
            : base(message, HttpStatusCode.BadRequest, details) { }

        public HttpBadRequestException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.BadRequest, details) { }
    }

    /// <summary>
    /// 表示未授权访问（HTTP 401）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 用户未登录或登录已过期
    /// 2. Token 无效或已过期
    /// 3. 需要登录才能访问的资源
    /// 示例：
    /// <code>
    /// throw new HttpUnauthorizedException("请先登录");
    /// throw new HttpUnauthorizedException("登录已过期");
    /// </code>
    /// </remarks>
    public class HttpUnauthorizedException : HttpApiException
    {
        public HttpUnauthorizedException(string message, string? details = null)
            : base(message, HttpStatusCode.Unauthorized, details) { }

        public HttpUnauthorizedException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.Unauthorized, details) { }
    }

    /// <summary>
    /// 表示访问被禁止（HTTP 403）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 用户已登录但权限不足
    /// 2. 访问其他用户的私有数据
    /// 3. 账户被禁用或冻结
    /// 4. IP 地址被限制访问
    /// 注意：与 401 的区别是用户身份已验证但权限不足
    /// 示例：
    /// <code>
    /// throw new HttpForbiddenException("您没有权限执行此操作");
    /// throw new HttpForbiddenException("您的账户已被禁用");
    /// </code>
    /// </remarks>
    public class HttpForbiddenException : HttpApiException
    {
        public HttpForbiddenException(string message, string? details = null)
            : base(message, HttpStatusCode.Forbidden, details) { }

        public HttpForbiddenException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.Forbidden, details) { }
    }

    /// <summary>
    /// 表示请求的资源不存在（HTTP 404）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 查询不存在的用户、订单等资源
    /// 2. 访问的 API 端点不存在
    /// 3. 文件或资源已被删除
    /// 示例：
    /// <code>
    /// throw new HttpNotFoundException($"用户 {userId} 不存在");
    /// throw new HttpNotFoundException("请求的订单不存在");
    /// </code>
    /// </remarks>
    public class HttpNotFoundException : HttpApiException
    {
        public HttpNotFoundException(string message, string? details = null)
            : base(message, HttpStatusCode.NotFound, details) { }

        public HttpNotFoundException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.NotFound, details) { }
    }

    /// <summary>
    /// 表示请求与服务器当前状态冲突（HTTP 409）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 创建资源时发生重复（如邮箱已注册）
    /// 2. 更新资源时版本冲突
    /// 3. 操作与当前资源状态冲突
    /// 示例：
    /// <code>
    /// throw new HttpConflictException("邮箱已被注册");
    /// throw new HttpConflictException("订单状态已更新，请刷新后重试");
    /// </code>
    /// </remarks>
    public class HttpConflictException : HttpApiException
    {
        public HttpConflictException(string message, string? details = null)
            : base(message, HttpStatusCode.Conflict, details) { }

        public HttpConflictException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.Conflict, details) { }
    }

    /// <summary>
    /// 表示请求格式正确，但由于语义错误无法处理（HTTP 422）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 数据验证失败（业务规则验证）
    /// 2. 无法处理的实体（如金额为负数）
    /// 3. 依赖关系不满足
    /// 注意：与 400 的区别是参数语法正确但业务逻辑不正确
    /// 示例：
    /// <code>
    /// throw new HttpUnprocessableEntityException("结束时间不能早于开始时间");
    /// throw new HttpUnprocessableEntityException("库存不足，无法完成订单");
    /// </code>
    /// </remarks>
    public class HttpUnprocessableEntityException : HttpApiException
    {
        public HttpUnprocessableEntityException(string message, string? details = null)
            : base(message, HttpStatusCode.UnprocessableEntity, details) { }

        public HttpUnprocessableEntityException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.UnprocessableEntity, details) { }
    }

    /// <summary>
    /// 表示请求频率过高（HTTP 429）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. API 调用频率超过限制
    /// 2. 防止暴力破解的攻击
    /// 3. 资源访问限流
    /// 示例：
    /// <code>
    /// throw new HttpTooManyRequestsException("请求过于频繁，请稍后重试");
    /// throw new HttpTooManyRequestsException("验证码发送次数超限，请1小时后再试");
    /// </code>
    /// </remarks>
    public class HttpTooManyRequestsException : HttpApiException
    {
        public HttpTooManyRequestsException(string message, string? details = null)
            : base(message, HttpStatusCode.TooManyRequests, details) { }

        public HttpTooManyRequestsException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.TooManyRequests, details) { }
    }

    /// <summary>
    /// 表示服务器内部错误（HTTP 500）
    /// </summary>
    /// <remarks>
    /// 使用场景：
    /// 1. 数据库连接失败
    /// 2. 外部服务调用失败
    /// 3. 未预期的系统异常
    /// 4. 配置错误或环境问题
    /// 注意：此异常应谨慎使用，通常用于捕获未处理的系统异常
    /// 示例：
    /// <code>
    /// throw new HttpInternalServerErrorException("系统繁忙，请稍后重试");
    /// throw new HttpInternalServerErrorException("数据处理失败，请联系管理员");
    /// </code>
    /// </remarks>
    public class HttpInternalServerErrorException : HttpApiException
    {
        /// <summary>
        /// 初始化 HttpInternalServerErrorException 类的新实例
        /// </summary>
        /// <param name="message">面向用户的错误消息</param>
        /// <param name="details">详细的错误信息</param>
        public HttpInternalServerErrorException(string message, string? details = null)
            : base(message, HttpStatusCode.InternalServerError, details) { }

        /// <summary>
        /// 初始化 HttpInternalServerErrorException 类的新实例（包含内部异常）
        /// </summary>
        /// <param name="message">面向用户的错误消息</param>
        /// <param name="innerException">导致当前异常的内部异常</param>
        /// <param name="details">详细的错误信息</param>
        public HttpInternalServerErrorException(string message, Exception innerException, string? details = null)
            : base(message, innerException, HttpStatusCode.InternalServerError, details) { }
    }
}