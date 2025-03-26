using Application.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly TelegramBotClient _botClient;

        public MessageService(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMessageAsync(
            long chatId, 
            string message, 
            ReplyMarkup? keyboard = null, 
            ParseMode parseMode = ParseMode.Html)
        {
            try 
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: message,
                    replyMarkup: keyboard,
                    parseMode: parseMode
                );
            }
            catch (Exception ex)
            {
                // Log the error or handle it appropriately
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public ReplyKeyboardMarkup CreateThemeKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Sports", "Technology", "Politics" },
                new KeyboardButton[] { "Business", "Entertainment", "Health" }
            })
            {
                ResizeKeyboard = true
            };
        }
    }
}