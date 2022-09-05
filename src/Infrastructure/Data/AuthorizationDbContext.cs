using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class AuthorizationDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Family> Families { get; set; }
    public virtual DbSet<FamilyRole> FamilyRoles { get; set; }
    public virtual DbSet<UserFamilyRole> UserFamilyRoles { get; set; }

    public AuthorizationDbContext() { }

    public AuthorizationDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFamilyRole>().HasKey(x => new { x.UserId, x.FamilyId });
        modelBuilder.Seed();
    }
}