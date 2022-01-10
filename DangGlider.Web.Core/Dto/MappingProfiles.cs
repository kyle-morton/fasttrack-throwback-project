using AutoMapper;
using DangGlider.Web.Core.Domain;

namespace DangGlider.Web.Core.Dto
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<GeoCode, GeoCodeDto>();
            CreateMap<Flight, FlightDto>();

            CreateMap<GeoCodeDto, GeoCode>();
            CreateMap<FlightDto, Flight>();
        }
    }
}
