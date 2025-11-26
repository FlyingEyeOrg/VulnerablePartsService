using FlyingEye.EntityFrameworkCore;
using FlyingEye.Spacers;
using FlyingEye.Spacers.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FlyingEye.Repositories
{
    internal class SpacerValidationDataRecordRepository
        : EfCoreRepository<VulnerablePartsServiceDbContext, SpacerValidationDataRecordModel, Guid>, ISpacerValidationDataRecordRepository
    {
        public SpacerValidationDataRecordRepository(IDbContextProvider<VulnerablePartsServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}