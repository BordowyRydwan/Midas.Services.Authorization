using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Midas.Services;
using Moq;
using UserRegisterDto = Application.Dto.UserRegisterDto;

namespace Application.UnitTests.Common;

[TestFixture]
public class UserRegisterTests
{
    private readonly IAuthorizationService _service;

    private IList<User> _data = new List<User>
    {
        new()
        {
            Id = 23,
            Email = "dawid.wijata@hotmail.com",
            Password = string.Empty
        }
    };

    public UserRegisterTests()
    {
        var mockRepository = new Mock<IUserRepository>();
        var mapper = AutoMapperConfig.Initialize();
        var mockPasswordHasher = new PasswordHasher<User>();
        var mockConfiguration = new Mock<IConfiguration>();
        var mockUserClient = new Mock<IUserClient>();

        _data[0].Password = mockPasswordHasher.HashPassword(_data[0], "zaq1@WSX");
        
        mockUserClient.Setup(x => x.RegisterNewUserAsync(It.IsAny<Midas.Services.UserRegisterDto>()))
            .ReturnsAsync((Midas.Services.UserRegisterDto dto) =>
            {
                if (_data.Any(x => x.Email == dto.Email)) return null;
                
                return new UserRegisterReturnDto
                {
                    Email = dto.Email,
                    Id = (long)(_data.Max(x => x.Id) + 1)
                };
            });
        mockRepository.Setup(x => x.AddNewUser(It.IsAny<User>()))
            .Callback<User>(entity =>
            {
                if (!_data.Any(x => x.Email == entity.Email))
                {
                    _data.Add(entity);
                }
            });

        _service = new AuthorizationService(mockRepository.Object, mockPasswordHasher, mapper, mockConfiguration.Object,
            mockUserClient.Object);
    }
    
    [TearDown]
    public void ClearList()
    {
        _data = new List<User>
        {
            new()
            {
                Id = 23,
                Email = "dawid.wijata@hotmail.com",
                Password = string.Empty
            }
        };
    }

    [Test]
    public async Task RegisterUser_IfEverythingGoesCorrect()
    {
        var dto = new UserRegisterDto
        {
            BirthDate = new DateTime(2000, 10, 12),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "Testowy",
            Password = "zaq1@WSX"
        };
        var result = await _service.RegisterNewUser(dto).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(_data.Count, Is.EqualTo(2));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(dto.Email));
            Assert.That(result.Id, Is.EqualTo((long)(_data.Max(x => x.Id) + 1)));
        });
    }
    
    [Test]
    public async Task DoNotRegisterUser_IfUserExists()
    {
        var dto = new UserRegisterDto
        {
            BirthDate = new DateTime(2000, 10, 12),
            Email = "dawid.wijata@hotmail.com",
            FirstName = "Test",
            LastName = "Testowy",
            Password = "zaq1@WSX",
        };
        var result = await _service.RegisterNewUser(dto).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(_data.Count, Is.EqualTo(1));
            Assert.That(result, Is.Null);
        });
    }
}