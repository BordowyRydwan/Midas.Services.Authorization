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
public class DeleteFamilyTests
{
    private readonly FamilyController _familyController;
    private readonly UserController _userController;

    private ulong _userId;
    private ulong _familyId;

    public DeleteFamilyTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("AuthorizationConnection");

        var dbOptions = new DbContextOptionsBuilder<AuthorizationDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new AuthorizationDbContext(dbOptions);
        
        var userRepository = new UserRepository(dbContext);
        var familyRepository = new FamilyRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var familyService = new FamilyService(familyRepository, null, mapper);
        var familyLogger = Mock.Of<ILogger<FamilyController>>();
        _familyController = new FamilyController(familyLogger, familyService);
        
        var passwordHasher = new PasswordHasher<User>();
        var userService = new UserService(userRepository, mapper, passwordHasher);
        var userLogger = Mock.Of<ILogger<UserController>>();
        _userController = new UserController(userLogger, userService);
    }
    
    [SetUp]
    public async Task Init()
    {
        var randomNumber = new Random().Next(100000, 10000000);
        var initialUser = new UserRegisterDto
        {
            Email = $"test{randomNumber}@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
            Password = "zaq1@WSX"
        };

        var userResult = await _userController.RegisterNewUser(initialUser).ConfigureAwait(false) as OkObjectResult;
        _userId = (userResult.Value as UserRegisterReturnDto).Id;
        
        var initialInstance = new AddNewFamilyDto
        {
            Name = $"Family {randomNumber}",
            FounderId = _userId
        };
        var familyResult = await _familyController.AddNewFamily(initialInstance).ConfigureAwait(false) as OkObjectResult;
        _familyId = (familyResult.Value as AddNewFamilyReturnDto).Id;
    }
    
    [Test]
    public async Task OnNonExistingFamily_ShouldThrowHTTP404()
    {
        var randomNumber = Convert.ToUInt64(new Random().Next(100000, 1000000));
        var response = await _familyController.DeleteFamily(randomNumber).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public async Task OnExistingFamily_ShouldThrowHTTP200()
    {
        var response = await _familyController.DeleteFamily(_familyId).ConfigureAwait(false);
        
        Assert.That(response, Is.InstanceOf<OkResult>());
    }
}