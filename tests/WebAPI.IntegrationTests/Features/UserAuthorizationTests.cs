using System.Text.RegularExpressions;
using Application.Dto;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class UserAuthorizationTests
{
    private readonly AuthorizationController _authorizationController;
    private readonly UserController _userController;

    public UserAuthorizationTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("AuthorizationConnection");

        var dbOptions = new DbContextOptionsBuilder<AuthorizationDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new AuthorizationDbContext(dbOptions);
        var repository = new UserRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();
        var passwordHasher = new PasswordHasher<User>();

        var authService = new AuthorizationService(repository, mapper, passwordHasher, configuration);
        var authLogger = Mock.Of<ILogger<AuthorizationController>>();
        
        var userService = new UserService(repository, mapper, passwordHasher);
        var userLogger = Mock.Of<ILogger<UserController>>();

        _authorizationController = new AuthorizationController(authLogger, authService);
        _userController = new UserController(userLogger, userService);
    }
    
    [SetUp]
    public async Task Init()
    {
        var initialInstance = new UserRegisterDto
        {
            Email = "test@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };
        
        await _userController.RegisterNewUser(initialInstance).ConfigureAwait(false);
    }

    [Test]
    public async Task ShouldGenerateValidJWT_OnCorrectEmailAndPassword()
    {
        var testInstance = new UserLoginDto
        {
            Email = "test@gmail.com",
            Password = "zaq1@WSX"
        };
        var response = await _authorizationController.AuthorizeUser(testInstance).ConfigureAwait(false);
        var result = (response as OkObjectResult).Value.ToString();
        
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.InstanceOf<OkObjectResult>());
            Assert.That(IsValidJwtToken(result), Is.True);
        });
    }
    
    [Test]
    public async Task ShouldThrowHTTP400_OnInCorrectEmail()
    {
        var testInstance = new UserLoginDto
        {
            Email = "test@gmailkljlk.com",
            Password = "zaq1@WSX"
        };
        var response = await _authorizationController.AuthorizeUser(testInstance).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<BadRequestResult>());
    }
    
    [Test]
    public async Task ShouldThrowHTTP400_OnInCorrectPassword()
    {
        var testInstance = new UserLoginDto
        {
            Email = "test@gmail.com",
            Password = "zaq1@WSXkljklj"
        };
        var response = await _authorizationController.AuthorizeUser(testInstance).ConfigureAwait(false);

        Assert.That(response, Is.InstanceOf<BadRequestResult>());
    }

    private bool IsValidJwtToken(string token)
    {
        return new Regex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_.+/=]*$").IsMatch(token);
    }
}