using Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace Application.Common.Interfaces;

public interface IMessageService
{
    /// <summary>
    /// Sends a message with optional keyboard and parsing mode
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="message">Text message to send</param>
    /// <param name="keyboard">Optional reply keyboard</param>
    /// <param name="parseMode">Optional parse mode (default is HTML)</param>
    /// <returns>Task representing the send operation</returns>
    Task SendMessageAsync(
        long chatId, 
        string message, 
        ReplyMarkup? keyboard = null, 
        ParseMode parseMode = ParseMode.Html
    );

    /// <summary>
    /// Creates a standard theme selection keyboard
    /// </summary>
    /// <returns>ReplyKeyboardMarkup with predefined themes</returns>
    ReplyKeyboardMarkup CreateThemeKeyboard();

    /// <summary>
    /// Creates an advanced keyboard with multiple options
    /// </summary>
    /// <param name="state">Current bot state</param>
    /// <returns>Customized keyboard markup</returns>
    ReplyKeyboardMarkup CreateAdvancedKeyboard(BotState state);

    /// <summary>
    /// Creates an inline keyboard for additional interactions
    /// </summary>
    /// <returns>Inline keyboard markup</returns>
    InlineKeyboardMarkup CreateInlineKeyboard();
}