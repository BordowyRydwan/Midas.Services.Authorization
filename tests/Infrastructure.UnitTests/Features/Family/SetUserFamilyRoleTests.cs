using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class SetUserFamilyRoleTests
{
    private readonly IFamilyRepository _repository;

    private readonly IList<UserFamilyRole> _mockUserFamilyRoles = new List<UserFamilyRole>
    {
        new()
        {
            User = new User
            {
                Id = 1,
                Email = "test@test.pl",
                FirstName = "Test 1",
                LastName = "Test 1",
                BirthDate = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow
            },
            UserId = 1,
            Family = new Family
            {
                Id = 1,
                Name = "Test 1"
            },
            FamilyId = 1,
            FamilyRole = new FamilyRole
            {
                Id = 1,
                Name = "Main administrator"
            },
            FamilyRoleId = 1
        }
    };

    public SetUserFamilyRoleTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _mockUserFamilyRoles.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.UserFamilyRoles).Returns(mockData.Object);
        mockData.Setup(x => x.FindAsync(It.IsAny<object>())).ReturnsAsync((object[] ids) =>
        {
            var userId = ids[0].GetType().GetProperty("UserId").GetValue(ids[0], null) as ulong?;
            var familyId = ids[0].GetType().GetProperty("FamilyId").GetValue(ids[0], null) as ulong?;
            
            return _mockUserFamilyRoles.FirstOrDefault(x => x.UserId == userId && x.FamilyId == familyId);
        });
        mockData.Setup(x => x.Update(It.IsAny<UserFamilyRole>()))
            .Callback<UserFamilyRole>(entity =>
            {
                _mockUserFamilyRoles.First(x => x.FamilyId == entity.FamilyId && x.UserId == entity.UserId)
                    .FamilyRoleId = entity.FamilyRoleId;
            });

        _repository = new FamilyRepository(mockContext.Object);
    }

    [Test]
    public async Task ShouldReturnTrue_ForExistingEntity()
    {
        var testInstance = new UserFamilyRole
        {
            FamilyId = 1,
            UserId = 1,
            FamilyRoleId = 3
        };
        var result = await _repository.SetUserFamilyRole(testInstance).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_mockUserFamilyRoles.First().FamilyRoleId, Is.EqualTo(3));
        });
    }
    
    [Test]
    [TestCase(1UL, 2UL)]
    [TestCase(2UL, 1UL)]
    [TestCase(3UL, 3UL)]
    public async Task ShouldReturnFalse_ForNonExistingEntity(ulong userId, ulong familyId)
    {
        var testInstance = new UserFamilyRole
        {
            FamilyId = familyId,
            UserId = userId,
            FamilyRoleId = 3
        };
        var result = await _repository.SetUserFamilyRole(testInstance).ConfigureAwait(false);
        
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_mockUserFamilyRoles, Has.Count.EqualTo(1));
        });
    }
}