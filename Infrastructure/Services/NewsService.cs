using Application.Common.Interfaces;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System.Text;

namespace Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly NewsApiClient _newsApiClient;
        private readonly string[] _predefinedThemes =
            ["Sports", "Technology", "Politics", "Business", "Entertainment", "Health"];

        public NewsService(string newsApiKey)
        {
            _newsApiClient = new NewsApiClient(newsApiKey);
        }

        public async Task<string> FetchNewsAsync(string theme)
        {
            var relevantDateTime = DateTime.UtcNow.AddDays(-3);
            
            try 
            {
                var articlesResponse = await _newsApiClient.GetEverythingAsync(new EverythingRequest
                {
                    Q = Uri.EscapeDataString(theme),
                    SortBy = SortBys.Popularity,
                    From = relevantDateTime,
                    PageSize = 3,
                });

                if (articlesResponse.Status == Statuses.Ok)
                {
                    var sb = new StringBuilder();
                    foreach (var article in articlesResponse.Articles)
                    {
                        sb.AppendLine($"<b>{article.Title}</b>");
                        sb.AppendLine($"<blockquote>{article.Description ?? "No description available."}</blockquote>");
                        sb.AppendLine($"<a href=\"{article.Url}\">Read more</a>");
                        sb.AppendLine();
                    }
                    return sb.ToString();
                }

                return "Sorry, I couldn't retrieve news at this time.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"News API Error: {ex.Message}");
                return "An error occurred while fetching news.";
            }
        }

        public bool IsPredefinedTheme(string theme)
        {
            return _predefinedThemes.Any(t => 
                t.Equals(theme, StringComparison.OrdinalIgnoreCase));
        }
    }
}