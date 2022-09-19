using Application.Dto;

namespace Application.Interfaces;

public interface IUserService : IInternalService
{
    public Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user);
    public Task<bool> CheckUserCredentials(UserLoginDto user);
    public Task<string> GenerateJwtToken(UserLoginDto user);
}