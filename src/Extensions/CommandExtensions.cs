using System;
using MMAEvents.TelegramBot.Commands;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot
{
    public static class CommandExtensions
    {
        public static Message GetMessage(this ICommand command, Update update)
        {
            if (update is null)
                throw new ArgumentNullException(nameof(update));

            if (update.Message is not null)
                return update.Message;
            else if (update.EditedMessage is not null)
                return update.EditedMessage;
            else
                return update.CallbackQuery.Message;
        }
    }
}