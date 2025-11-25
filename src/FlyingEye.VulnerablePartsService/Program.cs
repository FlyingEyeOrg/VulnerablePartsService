using Serilog;
using Serilog.Events;

namespace FlyingEye
{
    public class Program
    {
        public async static Task<int> Main(string[] args)
        {
            InitSerilog();

            Log.Information("Starting web host.");

            try
            {
                await RunAsync(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;
        }

        private static void InitSerilog()
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            // 将 Microsoft 命名空间下的日志级别设置为 Information，这意味着来自 Microsoft 命名空间的日志将按 Information 级别进行记录。
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            // 将 Microsoft.EntityFrameworkCore 命名空间下的日志级别设置为 Warning，
            // 这意味着来自 Microsoft.EntityFrameworkCore 命名空间的日志将按 Warning 级别进行记录。
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            // 将当前的日志上下文（LogContext）包含在每个日志事件中。
            .Enrich.FromLogContext()
            // 将日志记录到一个名为 "logs.txt" 的文件中。Async 方法表示日志写入是异步执行的，以提高性能。
            .WriteTo.Async(c => c.File(
                "logs/log-.txt",
                rollingInterval: RollingInterval.Day,       // 按天滚动（默认值，可省略）
                retainedFileCountLimit: 7,                  // 保留最近7天的日志
                fileSizeLimitBytes: 100_000_000,            // 单个文件最大100MB
                rollOnFileSizeLimit: true                   // 超过大小后创建新文件
            ))
            // 将日志输出到控制台。
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
        }

        private async static Task<int> RunAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
            {
                Args = args,

#if RELEASE
                EnvironmentName = Environments.Production
#else
                EnvironmentName = Environments.Development
#endif

            });

            // 配置 autofac 和 日志，添加 autofac 一定要在 AddApplicationAsync 之前。
            builder.Host.UseAutofac()
                        .UseSerilog();
            await builder.AddApplicationAsync<VulnerablePartsServiceModule>();

            var app = builder.Build();

            app.InitializeApplication();

            await app.RunAsync();

            return 0;
        }
    }
}
