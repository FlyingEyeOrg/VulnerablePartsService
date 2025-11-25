using FlyingEye.Spacers.Repositories;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace FlyingEye.VulnerablePartsServices
{
    public class VulnerablePartsServiceDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;

        private readonly ISpacerValidationDataRepository _spacerValidationDataRepository;

        public VulnerablePartsServiceDataSeedContributor(
            IGuidGenerator guid,
            ISpacerValidationDataRepository spacerValidationDataRepository
         )
        {
            _guidGenerator = guid;
            _spacerValidationDataRepository = spacerValidationDataRepository;
        }

        [UnitOfWork]
        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateSpacerValidationDataAsync();
        }

        public async Task CreateSpacerValidationDataAsync()
        {
            var model = await _spacerValidationDataRepository.FindAsync(item => item.ResourceId == "TestResourceId");

            if (model == null)
            {
                await _spacerValidationDataRepository.InsertAsync(new Spacers.SpacerValidationDataModel(
                    site: "HD",
                    resourceId: "2EFX1024",
                    @operator: "3439351074",
                    modelPn: "117.5AH",
                    date: "251107",
                    bigCoatingWidth: "374.4",
                    smallCoatingWidth: "188.8",
                    whiteSpaceWidth: "26",
                    aT11Width: "7.3",
                    thickness: "1.2",
                    aBSite: "A"));
            }
        }
    }
}
