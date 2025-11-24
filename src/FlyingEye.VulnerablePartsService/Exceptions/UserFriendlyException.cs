namespace FlyingEye.Exceptions
{
    public class UserFriendlyException : InvalidOperationException
    {
        public string? ErrorCode { get; } // 示例：未来可扩展错误码
        public UserFriendlyException(string message, string? errorCode = null)
            : base(message) => ErrorCode = errorCode;

        // 支持内部异常传递
        public UserFriendlyException(string message, Exception innerException, string? errorCode = null)
            : base(message, innerException) => ErrorCode = errorCode;
    }
}
