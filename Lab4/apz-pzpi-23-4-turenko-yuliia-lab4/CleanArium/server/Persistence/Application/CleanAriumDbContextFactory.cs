using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Persistence.Application;

public class CleanAriumDbContextFactory : IDesignTimeDbContextFactory<CleanAriumDbContext>
{
    public CleanAriumDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CleanArium"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CleanAriumDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            b => b.MigrationsAssembly("Persistence"));

        return new CleanAriumDbContext(optionsBuilder.Options);
    }
}