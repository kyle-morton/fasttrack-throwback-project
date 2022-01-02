using AutoMapper;
using DangGlider.FlightGen.Core.Domain;

namespace DangGlider.FlightGen.API.Dto
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<GeoCode, GeoCodeDto>();
            CreateMap<Flight, FlightDto>();
        }
    }
}
