using Application.Dto;

namespace Application.Interfaces;

public interface IUserService : IInternalService
{
    public Task<bool> RegisterNewUser(UserRegisterDto user);
}