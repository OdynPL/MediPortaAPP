using MediPorta.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace MediPorta.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tag> Tags { get; set; }
    }
}
