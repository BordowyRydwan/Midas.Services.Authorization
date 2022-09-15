using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    public Task<ulong> AddNewUser(User user);
}