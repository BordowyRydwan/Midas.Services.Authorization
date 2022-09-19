using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthorizationDbContext _dbContext;

    public UserRepository(AuthorizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ulong> AddNewUser(User user)
    {
        var doesNewUserEmailExist = _dbContext.Users.Select(x => x.Email).Contains(user.Email);

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new UserException("Mail address is empty!");
        }
        
        if (doesNewUserEmailExist)
        {
            throw new UserException("Mail address already exists!");
        }

        await _dbContext.AddAsync(user).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return user.Id;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);
    }

    public async Task<User> GetUserById(ulong id)
    {
        return await _dbContext.Users.FindAsync(id).ConfigureAwait(false);
    }
}