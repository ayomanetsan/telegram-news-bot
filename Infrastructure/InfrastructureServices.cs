using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Infrastructure;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<NewsStatisticsContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(NewsStatisticsContext).Assembly.FullName)
            )
        );

        services.AddSingleton<TelegramBotClient>(sp => 
            new TelegramBotClient(configuration["Telegram:BotToken"]));

        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<INewsService>(sp => 
            new NewsService(configuration["NewsAPI:Key"]));
        services.AddScoped<UserStatisticsService>();
        services.AddScoped<UserSessionService>();

        return services;
    }
}