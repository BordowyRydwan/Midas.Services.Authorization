using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class DeleteFamilyTests
{
    private readonly IFamilyRepository _repository;

    private IList<Family> _data = new List<Family>
    {
        new()
        {
            Id = 1,
            Name = "Test 1"
        }
    };
    
    public DeleteFamilyTests()
    {
        var mockContext = new Mock<AuthorizationDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();
        
        mockData.Setup(x => x.FindAsync(It.IsAny<ulong>())).ReturnsAsync((object[] ids) =>
        {
            var id = (ulong)ids[0];
            return _data.FirstOrDefault(x => x.Id == id);
        });
        mockData.Setup(x => x.Remove(It.IsAny<Family>())).Callback<Family>(family => _data.Remove(family));
        mockContext.Setup(x => x.Families).Returns(mockData.Object);
        
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
    public async Task DeleteFamilyWithExistingId_ShouldDeleteEntry()
    {
        var result = await _repository.DeleteFamily(1UL).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_data, Has.Count.Zero);
        });
    }
    
    [Test]
    public async Task DeleteFamilyWithNonExistingId_ShouldNotDeleteAnything()
    {
        var result = await _repository.DeleteFamily(2137UL).ConfigureAwait(false);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(_data, Has.Count.EqualTo(1));
        });
    }
}