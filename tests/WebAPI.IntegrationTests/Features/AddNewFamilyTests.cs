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
public class AddNewFamilyTests
{
    private readonly FamilyController _familyController;
    private readonly UserController _userController;

    public AddNewFamilyTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("AuthorizationConnection");

        var dbOptions = new DbContextOptionsBuilder<AuthorizationDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new AuthorizationDbContext(dbOptions);
        
        var userRepository = new UserRepository(dbContext);
        var familyRepository = new FamilyRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var familyService = new FamilyService(familyRepository, mapper);
        var familyLogger = Mock.Of<ILogger<FamilyController>>();
        _familyController = new FamilyController(familyLogger, familyService);
        
        var userService = new UserService(userRepository, mapper);
        var userLogger = Mock.Of<ILogger<UserController>>();
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
    public async Task OnEmptyFamilyName_ShouldThrowHTTP400()
    {
        var testInstance = new AddNewFamilyDto
        {
            Name = "",
            FounderId = 1
        };

        var response = await _familyController.AddNewFamily(testInstance).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public async Task OnNullFamilyName_ShouldThrowHTTP400()
    {
        var testInstance = new AddNewFamilyDto
        {
            FounderId = 1
        };

        var response = await _familyController.AddNewFamily(testInstance).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public async Task OnNonExistingUser_ShouldThrowHTTP404()
    {
        var randomNumber = new Random().Next(1000, 1000000);
        var testInstance = new AddNewFamilyDto
        {
            Name = $"Test {randomNumber}",
            FounderId = (ulong)randomNumber
        };

        var response = await _familyController.AddNewFamily(testInstance).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
    }
    
    [Test]
    public async Task OnProperModel_ShouldThrowHTTP200()
    {
        var randomNumber = new Random().Next(1000, 1000000);
        var testInstance = new AddNewFamilyDto
        {
            Name = $"Test {randomNumber}",
            FounderId = 1
        };

        var response = await _familyController.AddNewFamily(testInstance).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<OkObjectResult>());
    }
}