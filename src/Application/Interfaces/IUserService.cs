using Application.Dto;

namespace Application.Interfaces;

public interface IUserService : IInternalService
{
    public Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user);
    public Task<bool> UpdateUserData(UserUpdateDto user);
    public Task<bool> UpdateUserEmail(UserUpdateEmailDto user);
    public Task<bool> UpdateUserPassword(UserUpdatePasswordDto user);
}