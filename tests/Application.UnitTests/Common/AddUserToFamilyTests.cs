using Application.Dto;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interfaces;
using Moq;

namespace Application.UnitTests.Common;

[TestFixture]
public class AddUserToFamilyTests
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

    public AddUserToFamilyTests()
    {
        var userRepository = new Mock<IUserRepository>();
        var familyRepository = new Mock<IFamilyRepository>();
        var mapper = AutoMapperConfig.Initialize();

        userRepository.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => _users.FirstOrDefault(x => x.Email == email));
        familyRepository.Setup(x => x.GetFamilyById(It.IsAny<ulong>()))
            .ReturnsAsync((ulong id) => _families.FirstOrDefault(x => x.Id == id));
        familyRepository.Setup(x => x.AddUserToFamily(It.IsAny<ulong>(), It.IsAny<ulong>()))
            .ReturnsAsync((ulong userId, ulong familyId) => true);

        _familyService = new FamilyService(familyRepository.Object, userRepository.Object, mapper);
    }

    [Test]
    public async Task ShouldReturnTrue_ForExistingEntity()
    {
        var dto = new AddUserToFamilyDto
        {
            Email = "test@test.com",
            FamilyId = 1
        };

        var result = await _familyService.AddUserToFamily(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    [TestCase("test@test.com", 2ul)]
    [TestCase("tes2@test.com", 1ul)]
    [TestCase("test3@test.com", 3ul)]
    public async Task ShouldReturnFalse_ForNotExistingEntity(string email, ulong id)
    {
        var dto = new AddUserToFamilyDto
        {
            Email = email,
            FamilyId = id
        };

        var result = await _familyService.AddUserToFamily(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
}