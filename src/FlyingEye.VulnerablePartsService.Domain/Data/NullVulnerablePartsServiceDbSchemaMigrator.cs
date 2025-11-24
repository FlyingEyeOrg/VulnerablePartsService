using OpenId2Ids.Data;
using Volo.Abp.DependencyInjection;

namespace FlyingEye.Data
{
    /* This is used if database provider does't define
     * IOpenId2IdsDbSchemaMigrator implementation.
     */
    public class NullVulnerablePartsServiceDbSchemaMigrator : IVulnerablePartsServiceDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
