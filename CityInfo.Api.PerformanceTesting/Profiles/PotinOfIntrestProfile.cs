using AutoMapper;

namespace CityInfo.Api.Profiles
{
    public class PotinOfIntrestProfile : Profile
    {
        public PotinOfIntrestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointOfInterestForCrationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
        }        
    }
}
