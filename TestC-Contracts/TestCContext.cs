using Microsoft.EntityFrameworkCore;
using TestC_contracts.Entities;

namespace TestC_contracts;

public class TestCContext : DbContext
{
    public DbSet<WorkReport> WorkReports { get; set; }
    
    public DbSet<StateMachineTestCState> StateMachineStates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=CoffeeTestDB;Username=coffeetestuser;Password=coffeetestpass");
    }
}