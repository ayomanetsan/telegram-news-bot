namespace Data.Entities;

/// <summary>
/// Represents a user interaction with the bot
/// </summary>
public class UserInteraction
{
    public int Id { get; set; }

    public int TelegramUserId { get; set; }

    public TelegramUser User { get; set; }

    public string Theme { get; set; }

    public DateTimeOffset InteractionDate { get; set; }

    public string InteractionDetails { get; set; }
}