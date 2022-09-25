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

        result.CreateMap<Message, MessageDto>().ReverseMap();
        result.CreateMap<ICollection<Message>, MessageListDto>()
            .ForMember(dest => dest.Items, act => act.MapFrom(src => src))
            .ForMember(dest => dest.Count, act => act.MapFrom(src => src.Count));

        result.CreateMap<UserRegisterDto, User>()
            .ForMember(dest => dest.RegisterDate, act => act.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, act => act.Ignore())
            .ForMember(dest => dest.UserFamilyRoles, act => act.Ignore())
            .ForMember(dest => dest.Password, act => act.Ignore());
        
        result.CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.RegisterDate, act => act.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, act => act.Ignore())
            .ForMember(dest => dest.UserFamilyRoles, act => act.Ignore())
            .ForMember(dest => dest.Password, act => act.Ignore());

        result.CreateMap<UserLoginDto, UserCredentials>().ReverseMap();

        result.CreateMap<AddNewFamilyDto, Family>()
            .ForMember(dest => dest.Id, act => act.Ignore());
        
        result.CreateMap<SetUserFamilyRoleDto, UserFamilyRole>()
            .ForMember(dest => dest.Family, act => act.Ignore())
            .ForMember(dest => dest.FamilyRole, act => act.Ignore())
            .ForMember(dest => dest.User, act => act.Ignore())
            .ForMember(dest => dest.UserId, act => act.Ignore());
        
        result.CreateMap<Family, FamilyDto>();
        result.CreateMap<FamilyRole, FamilyRoleDto>();
        result.CreateMap<UserFamilyRole, UserFamilyRoleDto>()
            .ForMember(dest => dest.FamilyRole, act => act.MapFrom(src => src.FamilyRole))
            .ForMember(dest => dest.Family, act => act.MapFrom(src => src.Family));
        result.CreateMap<ICollection<UserFamilyRole>, UserFamilyRoleListDto>()
            .ForMember(dest => dest.Items, act => act.MapFrom(src => src))
            .ForMember(dest => dest.Count, act => act.MapFrom(src => src.Count));
        result.CreateMap<User, UserDto>();

        return result;
    }

    public static IMapper Initialize()
    {
        return new MapperConfiguration(Config).CreateMapper();
    }
}