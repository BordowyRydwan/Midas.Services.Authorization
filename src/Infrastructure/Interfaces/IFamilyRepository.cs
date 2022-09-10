using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IFamilyRepository
{
    public Task AddNewFamily(Family family, ulong founderId);
}