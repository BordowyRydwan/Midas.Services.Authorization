using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class MessageDataSeeder
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>().HasData(
            new Message { Id = 1, Content = "Test message 1" },
            new Message { Id = 2, Content = "Test message 2" },
            new Message { Id = 3, Content = "Test message 3" },
            new Message { Id = 4, Content = "Test message 4" },
            new Message { Id = 5, Content = "Test message 5" }
        );
    }
}