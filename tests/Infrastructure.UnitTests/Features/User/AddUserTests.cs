using Domain.Entities;
using Domain.Exceptions;
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
        mockData.Setup(x => x.FindAsync(It.IsAny<ulong>())).ReturnsAsync((object[] ids) =>
        {
            var id = (ulong)ids[0];
            return _data.FirstOrDefault(x => x.Id == id);
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
            }
        };
    }

    [Test]
    public async Task ShouldNotAddUserWhenEmailAlreadyExists()
    {
        var testInstance = new User
        {
            Email = "test@test.pl",
        };

        var exception = Assert.ThrowsAsync<UserException>(async () =>
            await _repository.AddNewUser(testInstance).ConfigureAwait(false));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("Mail address already exists!"));
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task ShouldAddUserWhenEmailDoesNotExist()
    {
        var testInstance = new User
        {
            Email = "test2@test.pl",
        };

        await _repository.AddNewUser(testInstance).ConfigureAwait(false);
        Assert.That(_data, Has.Count.EqualTo(2));
    }
}