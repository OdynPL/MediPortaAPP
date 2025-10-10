using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediPorta.Infrastructure.Factory
{
    public class TestDbConnectionFactory : IDbConnectionFactory
    {
        public DbContextOptions<AppDbContext> CreateDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDb");
            return optionsBuilder.Options;
        }
    }
}
