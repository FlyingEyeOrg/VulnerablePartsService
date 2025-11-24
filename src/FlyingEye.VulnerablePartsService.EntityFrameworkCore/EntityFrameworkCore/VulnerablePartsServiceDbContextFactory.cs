using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlyingEye.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class VulnerablePartsServiceDbContextFactory : IDesignTimeDbContextFactory<VulnerablePartsServiceDbContext>
    {
        public VulnerablePartsServiceDbContext CreateDbContext(string[] args)
        {
            VulnerablePartsServiceEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var connectionString = configuration.GetConnectionString("VulnerablePartsService");

            Console.WriteLine($"ConnectionString: ${connectionString}");

            var builder = new DbContextOptionsBuilder<VulnerablePartsServiceDbContext>()
                .UseMySql(connectionString, ServerVersion.Parse("8.1"));

            return new VulnerablePartsServiceDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FlyingEye.VulnerablePartsService.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
