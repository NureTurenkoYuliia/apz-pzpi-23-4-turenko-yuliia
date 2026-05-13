using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Persistence.Application;

public class CleanAriumDbContextFactory : IDesignTimeDbContextFactory<CleanAriumDbContext>
{
    public CleanAriumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CleanAriumDbContext>();
        optionsBuilder.UseSqlServer("DefaultConnection",
            builder => builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "CleanArium"));

        return new CleanAriumDbContext(optionsBuilder.Options);
    }
}