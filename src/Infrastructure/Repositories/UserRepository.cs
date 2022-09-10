using System.Net;
using Domain.Entities;
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
    
    public async Task<bool> AddNewUser(User user)
    {
        var doesNewUserEmailExist = _dbContext.Users.Select(x => x.Email).Contains(user.Email);

        if (user.Email is null || doesNewUserEmailExist)
        {
            return false;
        }

        await _dbContext.AddAsync(user).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        
        return true;
    }
}