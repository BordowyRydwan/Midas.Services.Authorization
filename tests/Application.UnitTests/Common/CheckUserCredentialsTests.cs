using Application.Dto;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.UnitTests.Common;

[TestFixture]
public class CheckUserCredentialsTests
{
    private readonly IUserService _service;

    private IList<User> _data = new List<User>
    {
        new()
        {
            Id = 23,
            Email = "dawid.wijata@hotmail.com",
            FirstName = "Dawid",
            LastName = "Wijata",
            BirthDate = new DateTime(1998, 10, 12),
            UserFamilyRoles = new List<UserFamilyRole>()
        }
    };
    
    public CheckUserCredentialsTests()
    {
        var mockRepository = new Mock<IUserRepository>();
        var mapper = AutoMapperConfig.Initialize();
        var mockPasswordHasher = new PasswordHasher<User>();
        var mockConfiguration = new Mock<IConfiguration>();

        _data[0].Password = mockPasswordHasher.HashPassword(_data[0], "zaq1@WSX");

        mockRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => _data.FirstOrDefault(x => x.Email == email));

        _service = new UserService(mockRepository.Object, mapper, mockPasswordHasher, mockConfiguration.Object);
    }

    [Test]
    public async Task ShouldReturnTrue_OnCorrectEmailAndPassword()
    {
        var testInstance = new UserLoginDto
        {
            Email = "dawid.wijata@hotmail.com",
            Password = "zaq1@WSX"
        };
        var result = await _service.CheckUserCredentials(testInstance).ConfigureAwait(false);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public async Task ShouldReturnFalse_OnCorrectEmailAndIncorrectPassword()
    {
        var testInstance = new UserLoginDto
        {
            Email = "dawid.wijata@hotmail.com",
            Password = "zaq1@WSXkllkjj"
        };
        var result = await _service.CheckUserCredentials(testInstance).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public async Task ShouldReturnFalse_OnIncorrectEmail()
    {
        var testInstance = new UserLoginDto
        {
            Email = "dawid.wijata@shit.com",
            Password = "zaq1@WSX"
        };
        var result = await _service.CheckUserCredentials(testInstance).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
}