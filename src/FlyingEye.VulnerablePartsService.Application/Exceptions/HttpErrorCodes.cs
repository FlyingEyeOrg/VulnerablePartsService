using System.Net;

namespace FlyingEye.Exceptions
{
    /// <summary>
    /// HTTP 错误码定义（语义化格式）
    /// </summary>
    public static class HttpErrorCodes
    {
        private const string Prefix = "HttpError:";

        // 400 相关错误
        public const string BadRequest = Prefix + "BadRequest";
        public const string InvalidInput = Prefix + "InvalidInput";
        public const string MissingRequiredField = Prefix + "MissingRequiredField";
        public const string ValidationFailed = Prefix + "ValidationFailed";
        public const string InvalidFormat = Prefix + "InvalidFormat";

        // 401 相关错误
        public const string Unauthorized = Prefix + "Unauthorized";
        public const string TokenExpired = Prefix + "TokenExpired";
        public const string TokenInvalid = Prefix + "TokenInvalid";
        public const string LoginRequired = Prefix + "LoginRequired";

        // 403 相关错误
        public const string Forbidden = Prefix + "Forbidden";
        public const string InsufficientPermissions = Prefix + "InsufficientPermissions";
        public const string AccessDenied = Prefix + "AccessDenied";
        public const string AccountDisabled = Prefix + "AccountDisabled";

        // 404 相关错误
        public const string NotFound = Prefix + "404";
        public const string ResourceNotFound = Prefix + "ResourceNotFound";
        public const string UserNotFound = Prefix + "UserNotFound";
        public const string FileNotFound = Prefix + "FileNotFound";

        // 409 相关错误
        public const string Conflict = Prefix + "Conflict";
        public const string ResourceExists = Prefix + "ResourceExists";
        public const string ConcurrentModification = Prefix + "ConcurrentModification";
        public const string VersionConflict = Prefix + "VersionConflict";

        // 422 相关错误
        public const string UnprocessableEntity = Prefix + "UnprocessableEntity";
        public const string BusinessRuleViolation = Prefix + "BusinessRuleViolation";
        public const string InvalidState = Prefix + "InvalidState";
        public const string PreconditionFailed = Prefix + "PreconditionFailed";

        // 429 相关错误
        public const string TooManyRequests = Prefix + "TooManyRequests";
        public const string RateLimitExceeded = Prefix + "RateLimitExceeded";
        public const string QuotaExceeded = Prefix + "QuotaExceeded";

        // 500 相关错误
        public const string InternalServerError = Prefix + "InternalServerError";
        public const string ServiceUnavailable = Prefix + "ServiceUnavailable";
        public const string DatabaseError = Prefix + "DatabaseError";
        public const string ExternalServiceError = Prefix + "ExternalServiceError";

        /// <summary>
        /// 根据 HTTP 状态码获取对应的默认错误码
        /// </summary>
        public static string GetDefaultErrorCode(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => BadRequest,
                HttpStatusCode.Unauthorized => Unauthorized,
                HttpStatusCode.Forbidden => Forbidden,
                HttpStatusCode.NotFound => NotFound,
                HttpStatusCode.Conflict => Conflict,
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity,
                HttpStatusCode.TooManyRequests => TooManyRequests,
                HttpStatusCode.InternalServerError => InternalServerError,
                _ => InternalServerError
            };
        }
    }
}