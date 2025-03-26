namespace Data.Entities;

/// <summary>
/// Represents a user of the Telegram bot
/// </summary>
public class TelegramUser
{
    public int Id { get; set; }

    public long TelegramUserId { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTimeOffset RegistrationDate { get; set; }

    public DateTimeOffset LastInteractionDate { get; set; }

    public int TotalInteractions { get; set; }

    public string PreferredTheme { get; set; }

    public int ThemeSearchCount { get; set; }
}
