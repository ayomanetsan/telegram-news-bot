using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class NewsStatisticsContextFactory : IDesignTimeDbContextFactory<NewsStatisticsContext>
    {
        public NewsStatisticsContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NewsStatisticsContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new NewsStatisticsContext(optionsBuilder.Options);
        }
    }
}