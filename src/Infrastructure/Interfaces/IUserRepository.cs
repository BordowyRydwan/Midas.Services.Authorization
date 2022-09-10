using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    public Task AddNewUser(User user);
}