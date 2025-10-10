using MediPorta.API;
using MediPorta.Domain.Entites;
using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Factory;
using MediPorta.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        // Set env to Integration Tests
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext i IDbConnectionFactory
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            var factoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnectionFactory));
            if (factoryDescriptor != null)
                services.Remove(factoryDescriptor);

            // Add new InMemory DbContext for tests
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Replace connection factory
            services.AddSingleton<IDbConnectionFactory, TestDbConnectionFactory>();

            // Fill database with example data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.Tags.Any())
            {
                db.Tags.AddRange(
                    new Tag { Name = "csharp", Count = 100, Percentage = 50m },
                    new Tag { Name = "dotnet", Count = 50, Percentage = 25m }
                );
                db.SaveChanges();
            }
        });
    }
}
