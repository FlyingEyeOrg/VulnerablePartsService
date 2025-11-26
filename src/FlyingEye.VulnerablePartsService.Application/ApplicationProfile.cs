using AutoMapper;
using FlyingEye.Spacers;
using FlyingEye.SpacerServices;

namespace FlyingEye
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<SpacerValidationData, SpacerValidationDataModel>()
                .ReverseMap();

            CreateMap<SpacerValidationDataResult, SpacerValidationDataModel>()
                .ReverseMap();

            CreateMap<SpacerValidationData, SpacerValidationDataRecordModel>()
               .ReverseMap();

            CreateMap<SpacerValidationDataResult, SpacerValidationDataRecordModel>()
                .ReverseMap();
        }
    }
}
