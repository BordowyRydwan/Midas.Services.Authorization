using Domain.Entities;
using Domain.Exceptions;
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

    public async Task<bool> UpdateUserData(User user)
    {
        var entity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == user.Email).ConfigureAwait(false);

        if (entity is null)
        {
            return false;
        }

        user.Id = entity.Id;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        
        return true;
    }

    public async Task<bool> UpdateUserEmail(string from, string to)
    {
        var entity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == from).ConfigureAwait(false);

        if (entity is null)
        {
            return false;
        }

        entity.Email = to;
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public async Task<bool> UpdateUserPassword(string userEmail, string passwordHash)
    {
        var entity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == userEmail).ConfigureAwait(false);

        if (entity is null)
        {
            return false;
        }

        entity.Password = passwordHash;
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }
}