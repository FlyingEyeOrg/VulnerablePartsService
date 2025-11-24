using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace FlyingEye.EntityFrameworkCore
{
    [DependsOn(typeof(VulnerablePartsServiceDomainModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreMySQLModule))]
    public class VulnerablePartsServiceEntityFrameworkCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            VulnerablePartsServiceEfCoreEntityExtensionMappings.Configure();
            base.PreConfigureServices(context);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 注册数据库上下文。
            context.Services.AddAbpDbContext<VulnerablePartsServiceDbContext>(options =>
            {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            var configuration = context.Services.GetConfiguration();
            var connectionString = configuration.GetConnectionString("VulnerablePartsService");
            Console.WriteLine($"ConnectionString：{connectionString}");

            Configure<AbpDbContextOptions>(options =>
            {
                /* The main point to change your DBMS.
                 * See also OpenId2IdsMigrationsDbContextFactory for EF Core tooling. */
                options.Configure(context =>
                {
                    context.DbContextOptions.UseMySql(connectionString, ServerVersion.Parse("8.1"), options =>
                    {
                        // 配置拆分查询
                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
                });
            });
        }
    }
}
