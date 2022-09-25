using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class UpdateUserPasswordTests
{
    private readonly IUserRepository _repository;
    
    private IList<User> _data = new List<User>
    {
        new()
        {
            Id = 1,
            Email = "test@test.pl",
            Password = string.Empty,
            FirstName = "Test 1",
            LastName = "Test 1",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow,
            UserFamilyRoles = new List<UserFamilyRole>()
        }
    };
    
    public UpdateUserPasswordTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.Users).Returns(mockData.Object);

        _repository = new UserRepository(mockContext.Object);
    }

    [TearDown]
    public void ClearUserPassword()
    {
        _data.First().Password = string.Empty;
    }
    
    [Test]
    public async Task ShouldReturnTrue_OnExistingEmail()
    {
        var email = "test@test.pl";
        var newPassword = "testowe";
        var fn = _repository.UpdateUserPassword;
            
        Assert.That(async () => await fn(email, newPassword), Throws.Nothing);
    }
    
    [Test]
    public async Task ShouldThrowUserException_OnNotExistingEmail()
    {
        var email = "testkljkljkj@test.pl";
        var newPassword = "testowe";
        var fn = _repository.UpdateUserPassword;
            
        Assert.That(async () => await fn(email, newPassword), Throws.Exception.TypeOf<UserException>());
    }
}