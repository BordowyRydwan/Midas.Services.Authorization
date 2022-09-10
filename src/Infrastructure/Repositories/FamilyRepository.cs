using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FamilyRepository : IFamilyRepository
{
    private readonly AuthorizationDbContext _dbContext;

    public FamilyRepository(AuthorizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddNewFamily(Family family, ulong founderId)
    {
        var user = await _dbContext.Users.FindAsync(founderId).ConfigureAwait(false);
        var doesFamilyExist = await _dbContext.Families.AnyAsync(x => x.Name == family.Name).ConfigureAwait(false);

        if (user is null)
        {
            throw new UserException($"Could not find user with id: {founderId}");
        }
        
        if (string.IsNullOrWhiteSpace(family.Name))
        {
            throw new FamilyException("Family name is empty");
        }
        
        if (doesFamilyExist)
        {
            throw new FamilyException($"Family with name {family.Name} already exists!");
        }

        await _dbContext.AddAsync(family).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        const ulong mainAdminRoleId = 1UL;
        var userFamilyRole = new UserFamilyRole
        {
            Family = family,
            FamilyRole = await _dbContext.FamilyRoles.SingleAsync(x => x.Id == mainAdminRoleId),
            User = user
        };

        await _dbContext.AddAsync(userFamilyRole).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}