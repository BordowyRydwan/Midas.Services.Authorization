using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    public Task<bool> AddNewUser(User user);
}