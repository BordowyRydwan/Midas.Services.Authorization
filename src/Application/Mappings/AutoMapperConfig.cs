using Application.Dto;
using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Application.Mappings;

public static class AutoMapperConfig
{
    private static MapperConfigurationExpression Config => GetConfig();

    private static MapperConfigurationExpression GetConfig()
    {
        var result = new MapperConfigurationExpression();

        result.CreateMap<UserRegisterDto, User>()
            .ForMember(dest => dest.Id, act => act.Ignore());
        result.CreateMap<UserRegisterDto, Midas.Services.UserRegisterDto>()
            .ForMember(
                dest => dest.BirthDate, 
                act => act.MapFrom(src => DateTimeOffset.Parse($"{src.BirthDate:yyyy-MM-ddTHH:mm:ss.fffZ}"))
            );
        
        return result;
    }

    public static IMapper Initialize()
    {
        return new MapperConfiguration(Config).CreateMapper();
    }
}