using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Interfaces;

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
}