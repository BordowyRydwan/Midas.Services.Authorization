using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user)
    {
        var userEntity = _mapper.Map<UserRegisterDto, User>(user);
        var returnModel = new UserRegisterReturnDto { Email = user.Email };

        userEntity.Password = _passwordHasher.HashPassword(userEntity, user.Password);
        
        try
        {
            returnModel.Id = await _userRepository.AddNewUser(userEntity).ConfigureAwait(false);
        }
        catch (UserException)
        {
            return null;
        }

        return returnModel;
    }

    public async Task<bool> UpdateUserData(UserUpdateDto user)
    {
        var userEntity = _mapper.Map<UserUpdateDto, User>(user);
        var updateResult = await _userRepository.UpdateUserData(userEntity);

        return updateResult;
    }

    public async Task UpdateUserEmail(UserUpdateEmailDto user)
    {
        await _userRepository.UpdateUserEmail(user.OldEmail, user.NewEmail);
    }

    public async Task UpdateUserPassword(UserUpdatePasswordDto user)
    {
        var userEntity = await _userRepository.GetUserByEmail(user.Email);
        
        if (userEntity is null)
        {
            throw new UserException("User identified by source email does not exist!");
        }
        
        var hash = _passwordHasher.HashPassword(userEntity, user.NewPassword);
        var verificationResult = _passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, user.NewPassword);

        if (verificationResult == PasswordVerificationResult.Success)
        {
            throw new PasswordException("Source and destination password are the same!");
        }

        await _userRepository.UpdateUserPassword(user.Email, hash);
    }

    public async Task<UserDto> GetUserByEmail(string email)
    {
        var entity = await _userRepository.GetUserByEmail(email).ConfigureAwait(false);
        var dto = _mapper.Map<User, UserDto>(entity);

        return dto;
    }
}