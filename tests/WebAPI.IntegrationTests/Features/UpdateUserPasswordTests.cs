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

namespace WebAPI.IntegrationTests.Features;

[TestFixture]
public class UpdateUserPasswordTests
{
    private readonly UserController _userController;

    private ulong _id;
    private string _email;

    public UpdateUserPasswordTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("AuthorizationConnection");

        var dbOptions = new DbContextOptionsBuilder<AuthorizationDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new AuthorizationDbContext(dbOptions);
        var repository = new UserRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();
        var passwordHasher = new PasswordHasher<User>();

        var userService = new UserService(repository, mapper, passwordHasher);
        var userLogger = Mock.Of<ILogger<UserController>>();
        
        _userController = new UserController(userLogger, userService);
    }

    [SetUp]
    public async Task Init()
    {
        var randomNumber = new Random().Next(1000, 1000000);
        var initialInstance = new UserRegisterDto
        {
            Email = $"test{randomNumber}@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };

        var result = await _userController.RegisterNewUser(initialInstance).ConfigureAwait(false) as OkObjectResult;
        _id = (result.Value as UserRegisterReturnDto).Id;
        _email = initialInstance.Email;
    }
    
    [Test]
    public async Task ShouldUpdateData_ForExistingUser()
    {
        var dto = new UserUpdatePasswordDto
        {
            Email = _email,
            NewPassword = "testpassword"
        };

        var result = await _userController.UpdateUserPassword(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.InstanceOf<OkResult>());
    }


    [Test]
    public async Task ShouldNotUpdatePassword_ForNotExistingUser()
    {
        var dto = new UserUpdatePasswordDto
        {
            Email = _email + "salt",
            NewPassword = "testpassword"
        };
        var result = await _userController.UpdateUserPassword(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
    
    [Test]
    public async Task ShouldNotUpdatePassword_ForSamePasswords()
    {
        var dto = new UserUpdatePasswordDto
        {
            Email = _email,
            NewPassword = "zaq1@WSX"
        };
        var result = await _userController.UpdateUserPassword(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}