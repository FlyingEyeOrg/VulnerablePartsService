using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlyingEye.Extensions
{
    public static class SwaggerExtensions
    {
        public static WebApplication UseVersionedSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                // 多版本控制
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    // 直接硬编码正确的路径
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"API {description.ApiVersion}");
                }
            });

            return app;
        }

        public static WebApplicationBuilder AddVersionedSwagger(
         this WebApplicationBuilder builder,
         Action<OpenApiInfo, ApiVersionDescription>? configureInfo = null)
        {
            // 必须添加（否则Swagger无法识别端点）
            builder.Services.AddEndpointsApiExplorer();
            // 配置 swagger 生成选项
            builder.Services.AddOptions<SwaggerGenOptions>()
                .PostConfigure<IApiVersionDescriptionProvider>((options, provider) =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var info = new OpenApiInfo
                        {
                            Title = "飞眼-易损件管理系统 API 文档",
                            Version = description.ApiVersion.ToString(),
                            Description = "飞眼易损件管理系统 API"
                        };

                        configureInfo?.Invoke(info, description);
                        options.SwaggerDoc(description.GroupName, info);
                    }
                });
            builder.Services.AddSwaggerGen();
            return builder;
        }
    }
}
