using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public static class AutoMapperConfig
{
    private static MapperConfigurationExpression Config => GetConfig();

    private static MapperConfigurationExpression GetConfig()
    {
        var result = new MapperConfigurationExpression();

        result.CreateMap<Message, MessageDto>().ReverseMap();
        result.CreateMap<ICollection<Message>, MessageListDto>()
            .ForMember(dest => dest.Items, act => act.MapFrom(src => src))
            .ForMember(dest => dest.Count, act => act.MapFrom(src => src.Count));

        result.CreateMap<UserRegisterDto, User>()
            .ForMember(dest => dest.RegisterDate, act => act.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, act => act.Ignore())
            .ForMember(dest => dest.UserFamilyRoles, act => act.Ignore());

        return result;
    }

    public static IMapper Initialize()
    {
        return new MapperConfiguration(Config).CreateMapper();
    }
}