using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Asp.Versioning;
using FlyingEye.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Modularity;

namespace FlyingEye
{
    [DependsOn(
    typeof(AbpExceptionHandlingModule),
    typeof(VulnerablePartsServiceApplicationModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule))]
    public class VulnerablePartsServiceModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ApiVersionExtensions.AddApiVersioning(context.Services);
            context.Services.ConfigExceptionToHttpErrors();
            context.Services.AddHttpClient();

            // 设置应用程序启动路径，一定是基于主函数所在的模块作为启动路径。
            var startPath = AppContext.BaseDirectory;

            if (string.IsNullOrWhiteSpace(startPath) || !Directory.Exists(startPath))
            {
                throw new InvalidOperationException("无效的启动路径！");
            }

            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var cors = configuration["App:CorsOrigins"]
                             ?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(o => o.RemovePostFix("/"))
                             .ToArray() ?? throw new InvalidOperationException("The App:CorsOrigins is null");
                    builder
                        .WithOrigins(
                         cors
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // 配置 Https 端口
            Configure<KestrelServerOptions>(options =>
            {
                var port = configuration["Https:Port"]
                       ?? throw new InvalidOperationException("The Https:Port is null or empty");

                if (!int.TryParse(port, out var intPort))
                {
                    throw new InvalidOperationException("The Https:Port is invalid");
                }

                options.ListenAnyIP(int.Parse(port), options =>
                {
                    var pfxFile = Path.Combine(AppContext.BaseDirectory, configuration["Https:Certificate"]
                        ?? throw new InvalidOperationException("The Https:Certificate is null or empty."));
                    var password = configuration["Https:Password"]
                        ?? throw new InvalidOperationException("The Https:Password is null or empty.");
                    if (!File.Exists(pfxFile)) throw new InvalidOperationException($"{pfxFile} is not found.");

                    var cert = new X509Certificate2(pfxFile, password);
                    options.UseHttps(cert);
                });

                options.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                    listenOptions.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                });
            });

            // 添加 JWT 认证
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var oauth = configuration.GetSection("OAuth");

                    // 必须是 https 访问
                    options.RequireHttpsMetadata = false;

                    // 您的 IdentityServer 的基地址
                    options.Authority = oauth["Authority"];

                    // 配置验证 issuer 
                    options.TokenValidationParameters.ValidIssuer = oauth["ValidIssuer"];
                    options.TokenValidationParameters.ValidateIssuer = true;
                    var kkk = oauth["ClientId"];
                    // access_token 使用的是 API 资源，可以在此处指定名称资源名称，然后允许 access_token 验证
                    // id_token 使用的是 ClientId 作为 aud，可以在此指定 ClientId，然后允许 id_token 验证
                    options.TokenValidationParameters.ValidAudiences = [oauth["Audience"], oauth["ClientId"],];
                    options.TokenValidationParameters.ValidateAudience = true;

                    var jwkFile = Path.Combine(startPath, configuration["SigningKey:Path"]
                        ?? throw new InvalidOperationException("The SigningKey:Path is null or empty."));

                    if (!File.Exists(jwkFile)) throw new InvalidOperationException($"{jwkFile} is not found.");

                    var jsonString = File.ReadAllText(jwkFile);
                    var jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey(jsonString);

                    options.TokenValidationParameters.IssuerSigningKey = jwk;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;

                    // 默认情况下 IdentityServer 发出一个 typ 标头，建议额外检查
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                });
#if DEBUG
            // 如果使用 swagger，最小 api 模型，应该添加这个中间件
            // 详见：https://stackoverflow.com/questions/71932980/what-is-addendpointsapiexplorer-in-asp-net-core-6/71933535#71933535
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "VulnerablePartsService", Version = "v1" });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(startPath, xmlFilename));

                // 添加 jwt 认证配置到 swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "at+jwt"
                };
                options.AddSecurityDefinition("Bearer", securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
#endif
            base.ConfigureServices(context);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            app.UseRouting();
            // 通常跨域在 Routing  后面
            app.UseCors();
#if DEBUG
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FlyingEyeWebApi");
                });
            }
#endif
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
            });

            base.OnApplicationInitialization(context);
        }
    }
}
