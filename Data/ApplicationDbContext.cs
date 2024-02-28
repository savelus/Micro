using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Link> Links { get; set; }
    
    private const string ConnectionString = "Host=postgres;Port=5432;Database=Links;Username=postgres;Password=root;CommandTimeout=300";

    public ApplicationDbContext() : base()
    {
        var str = Database.GetConnectionString();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(ConnectionString);
}