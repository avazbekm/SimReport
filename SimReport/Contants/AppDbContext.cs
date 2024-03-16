using Microsoft.EntityFrameworkCore;
using SimReport.Entities.Assets;
using SimReport.Entities.Cards;
using SimReport.Entities.Companies;
using SimReport.Entities.Users;

namespace SimReport.Contants;

public class AppDbContext : DbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Card> Cards { get; set; }
    DbSet<Company> Companies { get; set; }
    DbSet<Asset> Assets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string DB_CONNECTIONSTRING = $"Host={"localhost"};" +
            $"Database={"SimReportDB"};" +
            $"Port={"5432"};" +
            $"User ID={"postgres"};" +
            $"Password={"root"}";
        
        // Configure the connection string
        optionsBuilder.UseNpgsql(DB_CONNECTIONSTRING);
    }

}
