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
}