using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class PostgreSqlConnectionFactory : IDbConnectionFactory
{
	private readonly IConfiguration _configuration;

	public PostgreSqlConnectionFactory(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public DbContextOptions<AppDbContext> CreateDbContextOptions()
	{
		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		var connStr = _configuration.GetConnectionString("DefaultConnection");
		optionsBuilder.UseNpgsql(connStr);
		return optionsBuilder.Options;
	}
}
