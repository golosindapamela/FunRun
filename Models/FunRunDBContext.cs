using Fun_Run_Registration.Models;
using Microsoft.EntityFrameworkCore;

public class FunRunDBContext : DbContext
{
    public FunRunDBContext(DbContextOptions<FunRunDBContext> options) : base(options) { }

    public DbSet<Participant> Participants { get; set; }

    // Optionally, you can override OnModelCreating if you have specific configurations.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // You can add any custom configurations here, if needed.
    }
}
