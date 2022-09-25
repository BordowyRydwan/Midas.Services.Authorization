using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class DeleteUserFromFamilyTests
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

    public DeleteUserFromFamilyTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _mockUserFamilyRoles.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.UserFamilyRoles).Returns(mockData.Object);
        mockContext.Setup(m => m.UserFamilyRoles.Remove(It.IsAny<UserFamilyRole>()))
            .Callback<UserFamilyRole>(entity => _mockUserFamilyRoles.Remove(entity));
        
        mockData.Setup(x => x.FindAsync(It.IsAny<object>())).ReturnsAsync((object[] ids) =>
        {
            var userId = ids[0].GetType().GetProperty("userId").GetValue(ids[0], null) as ulong?;
            var familyId = ids[0].GetType().GetProperty("familyId").GetValue(ids[0], null) as ulong?;
            
            return _mockUserFamilyRoles.FirstOrDefault(x => x.UserId == userId && x.FamilyId == familyId);
        });

        _repository = new FamilyRepository(mockContext.Object);
    }

    [Test]
    public async Task ShouldReturnTrue_ForExistingEntity()
    {
        var userId = 1UL;
        var familyId = 1UL;
        var result = await _repository.DeleteUserFromFamily(userId, familyId).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_mockUserFamilyRoles, Has.Count.EqualTo(0));
        });
    }
    
    [Test]
    [TestCase(1UL, 2UL)]
    [TestCase(2UL, 1UL)]
    [TestCase(3UL, 3UL)]
    public async Task ShouldReturnFalse_ForNonExistingEntity(ulong userId, ulong familyId)
    {
        var result = await _repository.DeleteUserFromFamily(userId, familyId).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
}