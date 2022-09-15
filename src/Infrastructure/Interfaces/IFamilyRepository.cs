using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IFamilyRepository
{
    public Task<ulong> AddNewFamily(Family family, ulong founderId);
    public Task<bool> DeleteFamily(ulong id);
}