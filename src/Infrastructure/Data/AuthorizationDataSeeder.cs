using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class AuthorizationDataSeeder
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FamilyRole>().HasData(
            new FamilyRole { Id = 1UL, Name = "Main administrator" },
            new FamilyRole { Id = 2UL, Name = "Parent" },
            new FamilyRole { Id = 3UL, Name = "Child" }
        );
    }
}