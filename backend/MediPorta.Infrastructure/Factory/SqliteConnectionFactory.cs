using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqliteConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbContextOptions<AppDbContext> CreateDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connStr = _configuration.GetConnectionString("SqliteConnection");
        optionsBuilder.UseSqlite(connStr);
        return optionsBuilder.Options;
    }
}
