using Application.Common.Interfaces;
using Data.Enums;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// Assuming you have your DbContext here

namespace TelegramNewsBot;

internal abstract class Program
{
    private static ServiceProvider _serviceProvider;

    private static async Task Main()
    {
        // Configuration setup
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Program>()
            .Build();

        // Dependency Injection Setup
        var services = new ServiceCollection();

        // Add configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Add Infrastructure services
        services.AddInfrastructureServices(configuration);

        _serviceProvider = services.BuildServiceProvider();

        // Resolve Bot Client and start receiving
        var botClient = _serviceProvider.GetRequiredService<TelegramBotClient>();

        var me = await botClient.GetMe();
        Console.WriteLine($"Bot id: {me.Id}. Bot Name: {me.FirstName}");

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Bot is up and running. Press any key to exit");
        Console.ReadKey();
        await cts.CancelAsync();
    }

    private static async Task HandleUpdateAsync(
        ITelegramBotClient bot, 
        Update update, 
        CancellationToken cancellationToken)
    {
        // Ensure message is text
        if (update is not { Type: UpdateType.Message, Message.Text: not null })
            return;

        using var scope = _serviceProvider.CreateScope();
        
        // Resolve required services
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        var newsService = scope.ServiceProvider.GetRequiredService<INewsService>();
        var statsService = scope.ServiceProvider.GetRequiredService<UserStatisticsService>();
        var sessionService = scope.ServiceProvider.GetRequiredService<UserSessionService>();

        var message = update.Message;
        var chatId = message.Chat.Id;
        var messageText = message.Text;

        // Get or create user session
        var userSession = await sessionService.GetOrCreateSessionAsync(message.From.Id);

        // Determine next state based on user input
        var nextState = sessionService.DetermineNextState(userSession, messageText);
        userSession.CurrentState = nextState;

        // Handle different states
        switch (nextState)
        {
            case BotState.Start:
                await messageService.SendMessageAsync(
                    chatId, 
                    "Welcome! What would you like to do?",
                    messageService.CreateAdvancedKeyboard(BotState.Start)
                );
                break;

            case BotState.WaitingForTheme:
                await messageService.SendMessageAsync(
                    chatId, 
                    "Please select a news theme or enter your own:",
                    messageService.CreateAdvancedKeyboard(BotState.WaitingForTheme)
                );
                break;

            case BotState.Settings:
                await messageService.SendMessageAsync(
                    chatId, 
                    "Settings Menu:",
                    messageService.CreateAdvancedKeyboard(BotState.Settings)
                );
                break;

            // News theme selection
            default:
                // Update session
                userSession.LastSelectedTheme = messageText;
                await sessionService.UpdateSessionAsync(userSession);

                // Fetch and send news
                var newsResult = await newsService.FetchNewsAsync(messageText);
                    
                // Record user interaction
                await statsService.RecordUserInteraction(message, messageText);

                // Send news with theme keyboard and inline options
                await messageService.SendMessageAsync(
                    chatId, 
                    newsResult, 
                    messageService.CreateAdvancedKeyboard(BotState.ThemeSelected)
                );
                break;
        }
    }

    private static Task HandleErrorAsync(
        ITelegramBotClient bot, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}