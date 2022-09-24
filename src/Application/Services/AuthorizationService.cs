using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthorizationService(
        IUserRepository userRepository, 
        IMapper mapper, 
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration
    )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
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
        var userEntity = await _userRepository.GetUserByEmail(user.Email).ConfigureAwait(false);

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
            audience: _configuration["Jwt:Key"], 
            claims: claims, 
            expires: expires, 
            signingCredentials: cred
        );
        
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}