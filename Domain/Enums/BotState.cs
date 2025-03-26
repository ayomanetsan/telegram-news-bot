namespace Data.Enums;

/// <summary>
/// Represents the different states of user interaction with the bot
/// </summary>
public enum BotState
{
    /// <summary>
    /// Initial state or reset state
    /// </summary>
    Start,

    /// <summary>
    /// Waiting for user to select a news theme
    /// </summary>
    WaitingForTheme,

    /// <summary>
    /// User has selected a theme and received news
    /// </summary>
    ThemeSelected,

    /// <summary>
    /// User is in settings or preferences menu
    /// </summary>
    Settings,

    /// <summary>
    /// User wants more details about a news article
    /// </summary>
    ArticleDetails,

    /// <summary>
    /// User is browsing saved or favorite articles
    /// </summary>
    SavedArticles
}