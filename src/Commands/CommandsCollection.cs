using System;
using System.Collections.Generic;
using System.Linq;
using MMAEvents.ApiClients;
using MMAEvents.TelegramBot.Services;

namespace MMAEvents.TelegramBot.Commands
{
    public class CommandsCollection
    {
        private readonly Dictionary<string, ICommand> commands;

        public IReadOnlyCollection<string> CommandNames => commands.Keys;

        public CommandsCollection(IEventsApiClient apiClient, TelegramBotService telegramService)
        {
            commands = new Dictionary<string, ICommand>();
            var types = typeof(Startup).Assembly.GetTypes()
                .Where(type => typeof(ICommand).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass);

            foreach (var type in types)
            {
                var instance = (ICommand)Activator.CreateInstance(type, apiClient, telegramService);

                var name = instance.GetType().GetProperty("CommandName").GetValue(instance).ToString();

                commands.TryAdd(name, instance);
            }
        }

        public ICommand GetCommandOrDefault(string commandName)
        {
            var command = commands.GetValueOrDefault(commandName);

            if (command is null)
                command = commands.Values.FirstOrDefault(c => c is IDefaultCommand);

            return command;
        }

        public IEnumerable<IClientCommand> GetClientCommands()
        {
            return commands.Values.Where(c => c is IClientCommand).Select(c => c as IClientCommand);
        }
    }
}
