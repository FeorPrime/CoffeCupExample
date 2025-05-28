using contracts.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace contracts;

public class ExpContext : DbContext
{
    public DbSet<WorkReport> WorkReports { get; set; }
    public DbSet<TestStateMachineState> ProcessingStates { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=CoffeeTestDB;Username=coffeetestuser;Password=coffeetestpass");
    }
}

public static class Exts
{
    public static void AddDb(this IServiceCollection services)
    {
        services.AddDbContext<ExpContext>();
    }
}