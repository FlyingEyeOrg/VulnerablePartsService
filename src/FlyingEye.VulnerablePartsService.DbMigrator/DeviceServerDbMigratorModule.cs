using FlyingEye.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FlyingEye
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(VulnerablePartsServiceEntityFrameworkCoreModule)
        )]
    public class DeviceAppServiceDbMigratorModule : AbpModule
    {
    }
}
