using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class UpdateUserDataTests
{
    private User _user = new()
    {
        Id = 1,
        Email = "test@test.pl",
        FirstName = "Test 2",
        LastName = "Test 2",
        BirthDate = DateTime.UtcNow,
        RegisterDate = DateTime.UtcNow,
        UserFamilyRoles = new List<UserFamilyRole>()
    };
    
    private readonly IUserRepository _repository;

    private IList<User> _data = new List<User>
    {
        new()
        {
            Id = 1,
            Email = "test@test.pl",
            FirstName = "Test 1",
            LastName = "Test 1",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow,
            UserFamilyRoles = new List<UserFamilyRole>()
        }
    };
    
    public UpdateUserDataTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.Users).Returns(mockData.Object);
        _repository = new UserRepository(mockContext.Object);
    }

    [TearDown]
    public void ClearList()
    {
        _data = new List<User>
        {
            new()
            {
                Email = "test@test.pl",
                FirstName = "Test 1",
                LastName = "Test 1",
                BirthDate = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow,
                UserFamilyRoles = new List<UserFamilyRole>()
            }
        };
    }

    [Test]
    public async Task UpdatesData_ForExistingUser()
    {
        var user = new User
        {
            Email = "test@test.pl",
            FirstName = "Test 2",
            LastName = "Test 2",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow,
            UserFamilyRoles = new List<UserFamilyRole>()
        };

        var result = await _repository.UpdateUserData(user).ConfigureAwait(false);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public async Task DoesNotUpdateData_ForNonExistingUser()
    {
        var user = new User
        {
            Email = "test2137@test.pl",
            FirstName = "Test 2",
            LastName = "Test 2",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow,
            UserFamilyRoles = new List<UserFamilyRole>()
        };

        var result = await _repository.UpdateUserData(user).ConfigureAwait(false);
        
        Assert.That(result, Is.False);
    }
}