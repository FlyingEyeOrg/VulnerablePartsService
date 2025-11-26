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

        private readonly ISpacerValidationDataRecordRepository _spacerValidationDataRecordRepository;

        public VulnerablePartsServiceDataSeedContributor(
            IGuidGenerator guid,
            ISpacerValidationDataRepository spacerValidationDataRepository,
            ISpacerValidationDataRecordRepository spacerValidationDataRecordRepository)
        {
            _guidGenerator = guid;
            _spacerValidationDataRepository = spacerValidationDataRepository;
            _spacerValidationDataRecordRepository = spacerValidationDataRecordRepository;
        }

        [UnitOfWork]
        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateSpacerValidationDataAndRecordAsync();
        }

        public async Task CreateSpacerValidationDataAndRecordAsync()
        {
            // 创建一个验证数据
            var data = await _spacerValidationDataRepository.FindAsync(item => item.ResourceId == "2EFX1024" && item.ABSite == "A");

            if (data == null)
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

            // 创建一个验证数据记录
            var dataRecord = await _spacerValidationDataRecordRepository.FindAsync(item => item.ResourceId == "2EFX1024" && item.ABSite == "A");

            if (dataRecord == null)
            {
                await _spacerValidationDataRecordRepository.InsertAsync(new Spacers.SpacerValidationDataRecordModel(
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
