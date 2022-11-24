using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    public Task<ulong> AddNewUser(User user);
    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserById(ulong id);
    public Task UpdateUserPassword(string userEmail, string passwordHash);
    public Task UpdateUserEmail(string from, string to);

}