using System.IdentityModel.Tokens.Jwt;
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
using Midas.Services;
using UserUpdateEmailDto = Application.Dto.UserUpdateEmailDto;

namespace Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUserClient _userClient;

    public AuthorizationService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IMapper mapper,
        IConfiguration configuration,
        IUserClient userClient
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _configuration = configuration;
        _userClient = userClient;
    }
    
    public async Task<bool> CheckUserCredentials(UserLoginDto user)
    {
        var userEntity = await _userRepository.GetUserByEmail(user.Email).ConfigureAwait(false);

        if (userEntity is null)
        {
            return false;
        }
        
        var verificationResult = _passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, user.Password);

        return verificationResult == PasswordVerificationResult.Success;
    }

    public async Task<string> GenerateJwtToken(UserLoginDto user)
    {
        var userEntity = await _userClient.GetUserByEmailAsync(user.Email).ConfigureAwait(false);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userEntity.Id.ToString()),
            new (ClaimTypes.Name, $"{userEntity.FirstName} {userEntity.LastName}"),
            new (ClaimTypes.Email, $"{userEntity.Email}")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:ExpireDays"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"], 
            audience: _configuration["Jwt:Issuer"], 
            claims: claims, 
            expires: expires, 
            signingCredentials: cred
        );
        
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
    public async Task<UserRegisterReturnDto> RegisterNewUser(Dto.UserRegisterDto user)
    {
        var userEntity = _mapper.Map<Dto.UserRegisterDto, User>(user);
        var userAddInfoDto = _mapper.Map<Dto.UserRegisterDto, Midas.Services.UserRegisterDto>(user);
        var returnModel = new UserRegisterReturnDto();

        userEntity.Password = _passwordHasher.HashPassword(userEntity, user.Password);

        try
        {
            returnModel = await _userClient.RegisterNewUserAsync(userAddInfoDto).ConfigureAwait(false);
            await _userRepository.AddNewUser(userEntity).ConfigureAwait(false);
        }
        catch (UserException e)
        {
            Console.WriteLine(e);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        return returnModel;
    }
    
    public async Task UpdateUserEmail(UserUpdateEmailDto user)
    {
        await _userRepository.UpdateUserEmail(user.OldEmail, user.NewEmail);
    }
}