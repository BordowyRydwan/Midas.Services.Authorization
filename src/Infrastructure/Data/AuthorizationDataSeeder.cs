using Domain.Consts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class AuthorizationDataSeeder
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FamilyRole>().HasData(
            new FamilyRole { Id = (ulong)FamilyRoles.MainAdministrator, Name = "Main administrator" },
            new FamilyRole { Id = (ulong)FamilyRoles.Parent, Name = "Parent" },
            new FamilyRole { Id = (ulong)FamilyRoles.Child, Name = "Child" }
        );
    }
}