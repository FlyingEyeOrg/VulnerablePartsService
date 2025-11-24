using FlyingEye.EntityFrameworkCore;
using FlyingEye.Spacers;
using FlyingEye.Spacers.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FlyingEye.Repositories
{
    internal class SpacerValidationDataRepository
        : EfCoreRepository<VulnerablePartsServiceDbContext, SpacerValidationDataModel, Guid>, ISpacerValidationDataRepository
    {
        public SpacerValidationDataRepository(IDbContextProvider<VulnerablePartsServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}