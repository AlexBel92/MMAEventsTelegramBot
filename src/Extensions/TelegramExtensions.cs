using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot
{
    public static class TelegramExtensions
    {
        public static async Task SetMyCommandsAsync(this TelegramBotClient telegramBotClient, Assembly assembly, CancellationToken cancellationToken = default)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            var commands = new List<BotCommand>();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => typeof(MMAEvents.TelegramBot.Commands.IClientCommand).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var name = type.GetProperty("Name").GetValue(null).ToString();
                var description = type.GetProperty("Description").GetValue(null).ToString();

                commands.Add(new BotCommand() { Command = name, Description = description });
            }

            await telegramBotClient.SetMyCommandsAsync(commands, cancellationToken);
        }

        public static async Task SetMyCommandsAsync(this TelegramBotClient telegramBotClient, IEnumerable<IClientCommand> clientCommands, CancellationToken cancellationToken = default)
        {
            if (clientCommands is null)
                throw new ArgumentNullException(nameof(clientCommands));

            var botCommands = from command in clientCommands
                              select new BotCommand()
                              {
                                  Command = command.CommandName,
                                  Description = command.CommandDescription
                              };

            await telegramBotClient.SetMyCommandsAsync(botCommands.ToList(), cancellationToken);
        }
    }
}