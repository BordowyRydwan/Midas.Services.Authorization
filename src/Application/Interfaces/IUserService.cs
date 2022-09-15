using Application.Dto;

namespace Application.Interfaces;

public interface IUserService : IInternalService
{
    public Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user);
}