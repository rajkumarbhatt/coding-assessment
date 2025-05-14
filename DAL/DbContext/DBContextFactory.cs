using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.DbContext;

public class DBContextFactory : IDesignTimeDbContextFactory<DBContext>
{
    public DBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
        optionsBuilder.UseNpgsql("Host=localhost:5433;Database=test;Username=postgres;Password=Tatva@123");

        return new DBContext(optionsBuilder.Options);
    }
}