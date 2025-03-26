using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class NewsStatisticsContext : DbContext
{
    public NewsStatisticsContext(DbContextOptions<NewsStatisticsContext> options) : base(options)
    {
    }

    public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<UserInteraction> UserInteractions => Set<UserInteraction>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserSession>()
            .HasKey(us => us.TelegramUserId);
        
        modelBuilder.Entity<UserSession>()
            .Property(us => us.TempData)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
    }
}