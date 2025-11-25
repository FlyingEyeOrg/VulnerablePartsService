using Asp.Versioning;

namespace FlyingEye.Extensions
{
    public static class ApiVersionExtensions
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.AddAbpApiVersioning(options =>
            {
                options.ReportApiVersions = true; // 响应头显示支持的版本
                options.AssumeDefaultVersionWhenUnspecified = true; // 未指定版本时使用默认值
                options.DefaultApiVersion = new ApiVersion(1, 0); // 设置默认版本
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"), // 查询字符串
                    new HeaderApiVersionReader("x-api-version"), // HTTP 头部
                    new UrlSegmentApiVersionReader() // URL 路径
                );
            });

            return services;
        }
    }
}
