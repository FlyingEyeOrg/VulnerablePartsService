using Asp.Versioning;
using FlyingEye.Exceptions;
using Serilog;

namespace FlyingEye.Extensions
{
    public static class ServiceConfigExtensions
    {
        public static WebApplicationBuilder ConfigService(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            // 配置选项
            builder.ConfigOptions();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // 设置默认版本为 1.0
                options.AssumeDefaultVersionWhenUnspecified = true; // 允许隐式使用默认版本
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true; // 在响应头中添加支持的版本信息
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";       // Swagger 分组命名规则
                options.SubstituteApiVersionInUrl = true; // 自动替换路由中的版本占位符
            });

            // 配置 Swagger
            builder.AddVersionedSwagger();

            services.AddAutoMapper(options =>
            {
                options.AddProfile<AutoMapperProfile>();
            });
            services.AddSerilog();
            services.AddControllers();
            services.AddHttpClient();

            // 不启用跨域请求
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                var port = builder.Configuration["HttpPort"]
                ?? throw new UserFriendlyException("未配置 Http 端口", ErrorCodes.SystemError);
                options.ListenAnyIP(int.Parse(port));
            });

            return builder;
        }
    }
}
