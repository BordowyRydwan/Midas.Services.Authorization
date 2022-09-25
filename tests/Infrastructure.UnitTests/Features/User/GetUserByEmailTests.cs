using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class GetUserByEmailTests
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

    public GetUserByEmailTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.Users).Returns(mockData.Object);

        _repository = new UserRepository(mockContext.Object);
    }

    [Test]
    public async Task ShouldReturnNullObjectOnNonExistingEmail()
    {
        var email = "test2137@test.pl";
        var result = await _repository.GetUserByEmail(email).ConfigureAwait(false);
        
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task ShouldReturnUserObjectOnExistingEmail()
    {
        var email = "test@test.pl";
        var result = await _repository.GetUserByEmail(email).ConfigureAwait(false);
        
        Assert.That(result, Is.Not.Null);
    }
}