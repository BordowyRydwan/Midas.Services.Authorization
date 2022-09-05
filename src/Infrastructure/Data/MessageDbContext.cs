using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class MessageDbContext : DbContext
{
    public virtual DbSet<Message> Messages { get; set; }

    public MessageDbContext()
    {
    }

    public MessageDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Seed();
    }
}