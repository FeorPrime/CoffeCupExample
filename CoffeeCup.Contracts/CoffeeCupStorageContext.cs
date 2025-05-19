using CoffeeCup.Contracts.Resources;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCup.Contracts;

public class CoffeeCupStorageContext : DbContext
{
    public DbSet<ResourceEntity> Resources { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=CoffeeTestDB;Username=coffeetestuser;Password=coffeetestpass");
    }
}