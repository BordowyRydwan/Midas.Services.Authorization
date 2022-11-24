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
        return await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);
    }

    public async Task<User> GetUserById(ulong id)
    {
        return await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
    }

    public async Task UpdateUserPassword(string userEmail, string passwordHash)
    {
        var entity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == userEmail).ConfigureAwait(false);

        if (entity is null)
        {
            throw new UserException("User identified by source email does not exist!");
        }

        entity.Password = passwordHash;
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
    
    public async Task UpdateUserEmail(string from, string to)
    {
        var fromEntity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == from).ConfigureAwait(false);
        var toEntity = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == to).ConfigureAwait(false);

        if (from == to)
        {
            throw new UserException("Source and destination email are the same!");
        }

        if (fromEntity is null)
        {
            throw new UserException("User identified by source email does not exist!");
        }
        
        if (toEntity is not null)
        {
            throw new UserException("Other user uses a destination email!");
        }

        fromEntity.Email = to;
        _dbContext.Users.Update(fromEntity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}