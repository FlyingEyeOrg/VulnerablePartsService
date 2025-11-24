using FlyingEye.EntityFrameworkCore;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace FlyingEye
{
    [DependsOn(
        typeof(VulnerablePartsServiceEntityFrameworkCoreModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule))]
    public class VulnerablePartsServiceApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            this.Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<ApplicationProfile>();
            });

            base.ConfigureServices(context);
        }
    }
}
