using Application.Dto;
using Midas.Services;
using UserUpdateEmailDto = Application.Dto.UserUpdateEmailDto;

namespace Application.Interfaces;

public interface IAuthorizationService : IInternalService
{
    public Task<bool> CheckUserCredentials(UserLoginDto user);
    public Task<string> GenerateJwtToken(UserLoginDto user);
    public Task<UserRegisterReturnDto> RegisterNewUser(Dto.UserRegisterDto user);
    public Task UpdateUserEmail(UserUpdateEmailDto user);
}