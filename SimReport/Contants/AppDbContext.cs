using SimReport.Entities.Users;
using SimReport.Entities.Cards;
using SimReport.Entities.Assets;
using SimReport.Entities.Companies;
using Microsoft.EntityFrameworkCore;

namespace SimReport.Contants;

public class AppDbContext : DbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Card> Cards { get; set; }
    DbSet<Company> Companies { get; set; }
    DbSet<Asset> Assets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure the connection string
        optionsBuilder.UseNpgsql(DB_CONNECTIONSTRING);
    }

    public const string DB_CONNECTIONSTRING = $"Host={"localhost"};" +
        $"Port={"5432"};" +
        $"Database={"SimReportDB"};" +
        $"User ID={"postgres"};" +
        $"Password={"root"}";
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Card>()
            .HasOne(u => u.User)
            .WithMany(f => f.Cards)
            .HasForeignKey(f => f.UserId);
        

        modelBuilder.Entity<Card>()
            .HasOne(d => d.Company)
            .WithMany(t => t.Cards)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
