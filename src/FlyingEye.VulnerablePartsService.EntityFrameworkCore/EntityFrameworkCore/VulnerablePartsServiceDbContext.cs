using FlyingEye.Consts;
using FlyingEye.Spacers;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FlyingEye.EntityFrameworkCore
{
    public class VulnerablePartsServiceDbContext : AbpDbContext<VulnerablePartsServiceDbContext>
    {
        public VulnerablePartsServiceDbContext(DbContextOptions<VulnerablePartsServiceDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpacerValidationDataModel>(builder =>
            {
                builder.ToTable(VulnerablePartsServiceDbConsts.DbTablePrefix + "SpacerValidationData",
                    VulnerablePartsServiceDbConsts.DbSchema);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
