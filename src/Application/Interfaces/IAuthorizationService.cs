using Application.Dto;

namespace Application.Interfaces;

public interface IAuthorizationService : IInternalService
{
    public Task<bool> CheckUserCredentials(UserLoginDto user);
    public Task<string> GenerateJwtToken(UserLoginDto user);
}