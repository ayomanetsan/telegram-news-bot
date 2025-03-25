using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramNewsBot;

internal abstract class Program
{
    private static TelegramBotClient? _botClient;
    private static string? _botToken;
    private static string? _newsApiKey;

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Program>()
            .Build();

        _botToken = configuration["Telegram:BotToken"];
        _newsApiKey = configuration["NewsAPI:Key"];

        if (string.IsNullOrWhiteSpace(_botToken) || string.IsNullOrWhiteSpace(_newsApiKey))
        {
            Console.WriteLine("Please set the user secrets for Telegram:BotToken and NewsAPI:Key.");
            return;
        }

        _botClient = new TelegramBotClient(_botToken);
        var me = await _botClient.GetMe();
        Console.WriteLine($"Bot id: {me.Id}. Bot Name: {me.FirstName}");

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Bot is up and running. Press any key to exit");
        Console.ReadKey();
        await cts.CancelAsync();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update is { Type: UpdateType.Message, Message.Text: not null })
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a message: {messageText}");
            await ProcessNewsRequest(chatId, messageText);
        }
    }

    private static async Task ProcessNewsRequest(long chatId, string theme)
    {
        const string promptMessage = "Type in the theme to search for or select from the keyboard below:";
        if (!IsPredefinedTheme(theme))
        {
            await _botClient!.SendMessage(
                chatId: chatId,
                text: promptMessage,
                cancellationToken: CancellationToken.None,
                parseMode: ParseMode.Html
            );
        }

        var newsResult = await FetchNewsFromApi(theme);

        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Sports", "Technology", "Politics" },
            new KeyboardButton[] { "Business", "Entertainment", "Health" }
        })
        {
            ResizeKeyboard = true
        };

        await _botClient!.SendMessage(
            chatId: chatId,
            text: newsResult,
            replyMarkup: keyboard,
            cancellationToken: CancellationToken.None,
            parseMode: ParseMode.Html
        );
    }

    private static bool IsPredefinedTheme(string theme)
    {
        string[] predefinedThemes = { "Sports", "Technology", "Politics", "Business", "Entertainment", "Health" };
        foreach (var predefined in predefinedThemes)
        {
            if (theme.Equals(predefined, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static async Task<string> FetchNewsFromApi(string theme)
    {
        var relevantDateTime = DateTime.UtcNow.AddDays(-3);
        var newsApiClient = new NewsApiClient(_newsApiKey);
        var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
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

    private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}
