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

                // 一个设备，只能存在一个 A 面和 B 面垫片。
                builder.HasIndex(e => new { e.ResourceId, e.ABSite })
                .IsUnique()
                .HasDatabaseName("IX_SpacerValidationData_ResourceId_ABSite"); ;
            });

            modelBuilder.Entity<SpacerValidationDataRecordModel>(builder =>
            {
                builder.ToTable(VulnerablePartsServiceDbConsts.DbTablePrefix + "SpacerValidationDataRecords", 
                    VulnerablePartsServiceDbConsts.DbSchema);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
