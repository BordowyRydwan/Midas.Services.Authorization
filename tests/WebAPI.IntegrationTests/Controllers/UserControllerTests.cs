using Application.Dto;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers;
using Moq;

namespace WebAPI.IntegrationTests.Controllers;

[TestFixture]
public class UserControllerTests
{
    private readonly UserController _userController;

    public UserControllerTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("AuthorizationConnection");

        var dbOptions = new DbContextOptionsBuilder<AuthorizationDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new AuthorizationDbContext(dbOptions);
        var repository = new UserRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new UserService(repository, mapper);
        var logger = Mock.Of<ILogger<UserController>>();

        _userController = new UserController(logger, service);
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
    public async Task UniqueEmailShouldReturnHTTP200()
    {
        var randomNumber = new Random().Next(10, 1000000);
        var testInstance = new UserRegisterDto
        {
            Email = $"test{randomNumber}@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };

        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkResult>());
    }

    [Test]
    public async Task ExistingEmailShouldReturnHTTP400()
    {
        var testInstance = new UserRegisterDto
        {
            Email = $"test@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };
        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }
    
    [Test]
    public async Task InvalidModelShouldReturnHTTP400()
    {
        var testInstance = new UserRegisterDto
        {
            Email = $"test@@@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };
        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }
}