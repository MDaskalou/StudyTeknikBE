// Infrastructure/Persistence/DesignTime/AppDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Persistence.DesignTime
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string? connFromArgs = null;
            for (int i = 0; i < args?.Length; i++)
            {
                if (string.Equals(args[i], "--connection", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    connFromArgs = args[i + 1];
                    break;
                }
            }
            
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var conn = cfg.GetConnectionString("DefaultConnection")
                       ?? "Server=(localdb)\\MSSQLLocalDB;Database=StudyTeknikDB;Trusted_Connection=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn, sql => 
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "dbo"))
                .Options;

            return new AppDbContext(options);
        }
    }
}