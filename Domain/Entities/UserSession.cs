using Data.Enums;

namespace Data.Entities;

/// <summary>
/// Represents a user's current session and interaction state
/// </summary>
public class UserSession
{
    public long TelegramUserId { get; set; }
        
    /// <summary>
    /// Current state of user interaction
    /// </summary>
    public BotState CurrentState { get; set; } = BotState.Start;
        
    /// <summary>
    /// Last selected news theme
    /// </summary>
    public string? LastSelectedTheme { get; set; }
        
    /// <summary>
    /// Timestamp of last interaction
    /// </summary>
    public DateTimeOffset LastInteractionTime { get; set; } = DateTime.UtcNow;
        
    /// <summary>
    /// Number of consecutive interactions
    /// </summary>
    public int InteractionCount { get; set; } = 0;
        
    /// <summary>
    /// Temporary data for multi-step interactions
    /// </summary>
    public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
}