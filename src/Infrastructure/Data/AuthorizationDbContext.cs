using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class AuthorizationDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }

    public AuthorizationDbContext() { }

    public AuthorizationDbContext(DbContextOptions options) : base(options) { }
}