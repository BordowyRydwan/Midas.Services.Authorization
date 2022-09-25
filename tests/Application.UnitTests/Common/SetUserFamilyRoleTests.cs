using Application.Dto;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common;

[TestFixture]
public class SetUserFamilyRoleTests
{
    private readonly IFamilyService _familyService;

    private IList<User> _users = new List<User>
    {
        new()
        {
            Id = 1,
            Email = "test@test.com",
            FirstName = "Dawid",
            LastName = "Wijata",
            BirthDate = new DateTime(1998, 10, 12),
        }
    };

    private IList<Family> _families = new List<Family>
    {
        new()
        {
            Id = 1,
            Name = "Test 1"
        }
    };

    public SetUserFamilyRoleTests()
    {
        var userRepository = new Mock<IUserRepository>();
        var familyRepository = new Mock<IFamilyRepository>();
        var mapper = AutoMapperConfig.Initialize();

        userRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => _users.FirstOrDefault(x => x.Email == email));
        familyRepository.Setup(x => x.GetFamilyById(It.IsAny<ulong>()))
            .ReturnsAsync((ulong id) => _families.FirstOrDefault(x => x.Id == id));
        familyRepository.Setup(x => x.SetUserFamilyRole(It.IsAny<UserFamilyRole>()))
            .ReturnsAsync((UserFamilyRole entity) =>
            {
                var userExists = _users.Select(x => x.Id).Contains(entity.UserId);
                var familyExists = _users.Select(x => x.Id).Contains(entity.FamilyId);

                return userExists && familyExists;
            });

        _familyService = new FamilyService(familyRepository.Object, userRepository.Object, mapper);
    }

    [Test]
    public async Task ShouldReturnTrue_ForExistingEntity()
    {
        var dto = new SetUserFamilyRoleDto
        {
            Email = "test@test.com",
            FamilyId = 1,
            FamilyRoleId = 3
        };

        var result = await _familyService.SetUserFamilyRole(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    [TestCase("test@test.com", 2ul)]
    [TestCase("tes2@test.com", 1ul)]
    [TestCase("test3@test.com", 3ul)]
    public async Task ShouldReturnFalse_ForNotExistingEntity(string email, ulong id)
    {
        var dto = new SetUserFamilyRoleDto
        {
            Email = email,
            FamilyId = id,
            FamilyRoleId = 3
        };

        var result = await _familyService.SetUserFamilyRole(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
}