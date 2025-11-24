using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenId2Ids.Data;
using Volo.Abp.DependencyInjection;

namespace FlyingEye.EntityFrameworkCore
{
    public class EntityFrameworkCoreVulnerablePartsServiceDbSchemaMigrator
        : IVulnerablePartsServiceDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreVulnerablePartsServiceDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the OpenId2IdsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<VulnerablePartsServiceDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
