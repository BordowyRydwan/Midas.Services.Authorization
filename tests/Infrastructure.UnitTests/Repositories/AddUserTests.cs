using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class AddUserTests
{
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

    public AddUserTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Users).Returns(mockData.Object);
        mockContext.Setup(m => m.AddAsync(It.IsAny<User>(), default))
            .Callback<User, CancellationToken>((user, _) =>
            {
                user.Id = (ulong)(_data.Count + 1);
                _data.Add(user);
            });

        _repository = new UserRepository(mockContext.Object);
    }
    
    [TearDown]
    public void ClearList()
    {
        _data = new List<User>
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
    }

    [Test]
    public async Task ShouldNotAddUserWhenEmailAlreadyExists()
    {
        var testInstance = new User
        {
            Email = "test@test.pl",
            FirstName = "Test 1",
            LastName = "Test 1",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow
        };
        
        var result = await _repository.AddNewUser(testInstance).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }
    
    [Test]
    public async Task ShouldAddUserWhenEmailDoesNotExist()
    {
        var testInstance = new User
        {
            Email = "test2@test.pl",
            FirstName = "Test 1",
            LastName = "Test 1",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow
        };
        
        var result = await _repository.AddNewUser(testInstance).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_data, Has.Count.EqualTo(2));
        });
    }
}