using MediPorta.Application.Interfaces;
using MediPorta.Application.Services;
using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Factory;
using MediPorta.Infrastructure.Interfaces;
using MediPorta.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Check if we run DB context for tests or regular DB
        var isIntegrationTest = builder.Environment.EnvironmentName == "IntegrationTests";

        if (isIntegrationTest)
        {
            // InMemory DB for IntegrationTests
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            builder.Services.AddSingleton<IDbConnectionFactory, TestDbConnectionFactory>();
        }
        else
        {
            // PostgreSQL for regular config
            builder.Services.AddSingleton<IDbConnectionFactory, PostgreSqlConnectionFactory>();
            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var factory = sp.GetRequiredService<IDbConnectionFactory>();
                var dbOptions = factory.CreateDbContextOptions();
                options.UseNpgsql(dbOptions.Extensions
                    .OfType<Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.NpgsqlOptionsExtension>()
                    .First().ConnectionString);
            });
        }

        // Repositories & Services
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IStackOverflowClient, StackOverflowClient>();
        builder.Services.AddScoped<ITagProcessor, TagProcessor>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();

        // AutoMapper
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Controllers & Swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient("StackOverflow", (sp, client) =>
        {
            client.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "MediPortaApp/1.0 (https://github.com/twoj-repo)");
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.Database.IsInMemory())
            {
                var retryCount = 5;
                var delay = TimeSpan.FromSeconds(5);

                for (int i = 0; i < retryCount; i++)
                {
                    try
                    {
                        db.Database.Migrate();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == retryCount - 1) throw;
                        Console.WriteLine($"Migration failed. Retry in {delay.Seconds}s. Error: {ex.Message}");
                        Thread.Sleep(delay);
                    }
                }
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowFrontend");
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}