namespace Application.Common.Interfaces;

public interface INewsService
{
    /// <summary>
    /// Fetches news articles for a given theme
    /// </summary>
    /// <param name="theme">News theme to search</param>
    /// <returns>Formatted news articles string</returns>
    Task<string> FetchNewsAsync(string theme);

    /// <summary>
    /// Validates if the theme is a predefined theme
    /// </summary>
    /// <param name="theme">Theme to validate</param>
    /// <returns>True if theme is predefined, false otherwise</returns>
    bool IsPredefinedTheme(string theme);
}