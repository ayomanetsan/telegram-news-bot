using Application.Common.Models;
using Data;
using Data.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Infrastructure.Services;

public class UserStatisticsService
{
    private readonly NewsStatisticsContext _context;

    public UserStatisticsService(NewsStatisticsContext context)
    {
        _context = context;
    }

    public async Task RecordUserInteraction(Message message, string theme)
    {
        // Find or create user
        var telegramUser = await _context.TelegramUsers
            .FirstOrDefaultAsync(u => u.TelegramUserId == message.From.Id);

        if (telegramUser == null)
        {
            telegramUser = new TelegramUser
            {
                TelegramUserId = message.From.Id,
                Username = message.From.Username ?? "Unknown",
                FirstName = message.From.FirstName,
                LastName = message.From.LastName ?? string.Empty,
                RegistrationDate = DateTime.UtcNow,
                LastInteractionDate = DateTime.UtcNow,
                TotalInteractions = 1,
                PreferredTheme = theme,
                ThemeSearchCount = 1
            };
            _context.TelegramUsers.Add(telegramUser);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Update existing user
            telegramUser.LastInteractionDate = DateTime.UtcNow;
            telegramUser.TotalInteractions++;
        
            // Update preferred theme logic
            if (telegramUser.PreferredTheme != theme)
            {
                telegramUser.ThemeSearchCount++;
            }
            telegramUser.PreferredTheme = theme;
            await _context.SaveChangesAsync();
        }

        // Record interaction
        var interaction = new UserInteraction
        {
            TelegramUserId = telegramUser.Id,
            Theme = theme,
            InteractionDate = DateTime.UtcNow,
            InteractionDetails = $"Searched news for theme: {theme}"
        };
        _context.UserInteractions.Add(interaction);

        await _context.SaveChangesAsync();
    }

    public async Task<UserStatistics> GetOverallStatistics()
    {
        return new UserStatistics
        {
            TotalUsers = await _context.TelegramUsers.CountAsync(),
            TotalInteractions = await _context.UserInteractions.CountAsync(),
            ThemePopularity = await _context.UserInteractions
                .GroupBy(u => u.Theme)
                .Select(g => new ThemePopularity 
                { 
                    Theme = g.Key, 
                    Count = g.Count() 
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync()
        };
    }
}
