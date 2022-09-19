using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class GetUserByIdTests
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

    public GetUserByIdTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.Users).Returns(mockData.Object);
        mockData.Setup(x => x.FindAsync(It.IsAny<ulong>())).ReturnsAsync((object[] ids) =>
        {
            var id = (ulong)ids[0];
            return _data.FirstOrDefault(x => x.Id == id);
        });

        _repository = new UserRepository(mockContext.Object);
    }

    [Test]
    public async Task ShouldReturnNullObjectOnNonExistingId()
    {
        var id = 2UL;
        var result = await _repository.GetUserById(id).ConfigureAwait(false);
        
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public async Task ShouldReturnUserObjectOnExistingId()
    {
        var id = 1UL;
        var result = await _repository.GetUserById(id).ConfigureAwait(false);
        
        Assert.That(result, Is.Not.Null);
    }
}