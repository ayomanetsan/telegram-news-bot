using Data.Entities;
using Data.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserSessionService
{
    private readonly NewsStatisticsContext _context;

    public UserSessionService(NewsStatisticsContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get or create a user session
    /// </summary>
    public async Task<UserSession> GetOrCreateSessionAsync(long telegramUserId)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.TelegramUserId == telegramUserId);

        if (session == null)
        {
            session = new UserSession { TelegramUserId = telegramUserId };
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
        }

        return session;
    }

    /// <summary>
    /// Update user session state
    /// </summary>
    public async Task UpdateSessionAsync(UserSession session)
    {
        session.LastInteractionTime = DateTime.UtcNow;
        session.InteractionCount++;
            
        _context.UserSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Determine next state based on user input
    /// </summary>
    public BotState DetermineNextState(UserSession session, string userInput)
    {
        return userInput switch
        {
            "🔍 Find News" => BotState.WaitingForTheme,
            "⚙️ Settings" => BotState.Settings,
            "🏠 Main Menu" => BotState.Start,
            "❤️ Saved Articles (WIP)" => BotState.Start,
            "🔄 More News (WIP)" => BotState.Start,
            "💾 Save Articles (WIP)" => BotState.Start,
            "📝 Change Preferences (WIP)" => BotState.Start,
            "🔔 Notification Settings (WIP)" => BotState.Start,
            "📖 Read Full Article (WIP)" => BotState.Start,
            "💾 Save Article (WIP)" => BotState.Start,
            "🔍 More Like This (WIP)" => BotState.Start,
            "📊 My Stats (WIP)" => BotState.Start,
            "/start" => BotState.Start,
            _ => BotState.ThemeSelected
        };
    }
}