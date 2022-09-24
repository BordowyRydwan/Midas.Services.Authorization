using Domain.Entities;
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
        var result = await _repository.UpdateUserPassword(email, newPassword).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_data.First().Password, Is.EqualTo(newPassword));
        });
    }
    
    [Test]
    public async Task ShouldReturnFalse_OnNotExistingEmail()
    {
        var email = "testkljkljkj@test.pl";
        var oldPassword = _data.First().Password;
        var newPassword = "testowe";
        var result = await _repository.UpdateUserPassword(email, newPassword).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_data.First().Password, Is.EqualTo(oldPassword));
        });
    }
}