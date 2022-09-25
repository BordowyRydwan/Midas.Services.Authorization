using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class AddNewFamilyTests
{
    private readonly IFamilyRepository _repository;

    private readonly IList<User> _mockUsers = new List<User>
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
    private readonly IList<FamilyRole> _mockFamilyRoles = new List<FamilyRole>
    {
        new()
        {
            Id = 1,
            Name = "Main administrator"
        }
    };

    private IList<Family> _data = new List<Family>
    {
        new()
        {
            Id = 1,
            Name = "Test 1"
        }
    };

    public AddNewFamilyTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        var mockUsers = _mockUsers.AsQueryable().BuildMockDbSet();
        var mockFamilyRoles = _mockFamilyRoles.AsQueryable().BuildMockDbSet();
        
        mockContext.Setup(x => x.Families).Returns(mockData.Object);
        mockContext.Setup(x => x.Users).Returns(mockUsers.Object);
        mockContext.Setup(x => x.FamilyRoles).Returns(mockFamilyRoles.Object);
        mockContext.Setup(m => m.AddAsync(It.IsAny<Family>(), default))
            .Callback<Family, CancellationToken>((family, _) =>
            {
                family.Id = (ulong)(_data.Count + 1);
                _data.Add(family);
            });
        
        mockContext.Setup(m => m.AddAsync(It.IsAny<UserFamilyRole>(), default))
            .Callback<UserFamilyRole, CancellationToken>((userFamilyRole, _) =>
            {
                _mockUsers.Single(x => x.Id == 1).UserFamilyRoles.Add(userFamilyRole);
            });
        
        mockUsers.Setup(x => x.FindAsync(It.IsAny<ulong>())).ReturnsAsync((object[] ids) =>
        {
            var id = (ulong)ids[0];
            return _mockUsers.FirstOrDefault(x => x.Id == id);
        });

        _repository = new FamilyRepository(mockContext.Object);
    }
    
    [TearDown]
    public void ClearList()
    {
        _data = new List<Family>
        {
            new()
            {
                Id = 1,
                Name = "Test 1"
            }
        };
    }

    [Test]
    public async Task AddFamilyWithEmptyName_ShouldThrowFamilyException()
    {
        var testInstance = new Family { Name = "" };
        var result = Assert.ThrowsAsync<FamilyException>(() => _repository.AddNewFamily(testInstance, 1));
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<FamilyException>());
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }
    
    [Test]
    public async Task AddFamilyWithExistingName_ShouldThrowFamilyException()
    {
        var testInstance = new Family { Name = "Test 1" };
        var result = Assert.ThrowsAsync<FamilyException>(() => _repository.AddNewFamily(testInstance, 1));
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<FamilyException>());
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }
    
    [Test]
    public async Task AddFamilyWithNotExistingUser_ShouldThrowUserException()
    {
        var testInstance = new Family { Name = "Test 2" };
        var result = Assert.ThrowsAsync<UserException>(() => _repository.AddNewFamily(testInstance, 2137));
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<UserException>());
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task AddFamilyWithProperModel_ShouldGoOK()
    {
        var testInstance = new Family { Name = "Test 2" };
        
        await _repository.AddNewFamily(testInstance, 1).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(_mockUsers.Single(x => x.Id == 1).UserFamilyRoles, Has.Count.EqualTo(1));
            Assert.That(_data, Has.Count.EqualTo(2));
        });
    }
}