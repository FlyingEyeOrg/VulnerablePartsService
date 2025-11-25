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
        }
    }
}
