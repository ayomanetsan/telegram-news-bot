using Application.Common.Interfaces;
using Data.Enums;
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
        
        public ReplyKeyboardMarkup CreateAdvancedKeyboard(BotState state)
        {
            return state switch
            {
                BotState.Start => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "🔍 Find News", "⚙️ Settings" },
                    new KeyboardButton[] { "❤️ Saved Articles (WIP)", "📊 My Stats (WIP)" }
                }) { ResizeKeyboard = true },

                BotState.WaitingForTheme => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "Sports", "Technology", "Politics" },
                    new KeyboardButton[] { "Business", "Entertainment", "Health" },
                    new KeyboardButton[] { "🏠 Main Menu" }
                }) { ResizeKeyboard = true },

                BotState.ThemeSelected => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "🔄 More News (WIP)", "💾 Save Articles (WIP)" },
                    new KeyboardButton[] { "🏠 Main Menu" }
                }) { ResizeKeyboard = true },

                BotState.Settings => new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "📝 Change Preferences (WIP)", "🔔 Notification Settings (WIP)" },
                    new KeyboardButton[] { "🏠 Main Menu" }
                }) { ResizeKeyboard = true },

                _ => CreateThemeKeyboard()
            };
        }
        
        public InlineKeyboardMarkup CreateInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Read Full Article (WIP)", "read_full_article"),
                    InlineKeyboardButton.WithCallbackData("💾 Save Article (WIP)", "save_article")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔍 More Like This (WIP)", "similar_articles")
                }
            });
        }
    }
}