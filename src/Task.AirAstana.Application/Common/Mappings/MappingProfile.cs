using AutoMapper;
using Task.AirAstana.Application.Features.Auth.Models.DTOs;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Enums;

namespace Task.AirAstana.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Flight, FlightDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<FlightStatus, string>().ConvertUsing(src => src.ToString());

        CreateMap<User, UserDto>();
        CreateMap<Role, RoleDto>();
    }
}