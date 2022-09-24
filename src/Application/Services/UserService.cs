using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Text;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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

    public async Task<bool> UpdateUserEmail(UserUpdateEmailDto user)
    {
        return await _userRepository.UpdateUserEmail(user.OldEmail, user.NewEmail);
    }

    public async Task<bool> UpdateUserPassword(UserUpdatePasswordDto user)
    {
        var hash = _passwordHasher.HashPassword(new User(), user.NewPassword);
        var updateResult = await _userRepository.UpdateUserPassword(user.Email, hash);

        return updateResult;
    }
}