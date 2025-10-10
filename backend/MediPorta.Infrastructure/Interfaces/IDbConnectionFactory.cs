using MediPorta.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MediPorta.Infrastructure.Interfaces
{
    public interface IDbConnectionFactory
    {
        DbContextOptions<AppDbContext> CreateDbContextOptions();
    }

}
