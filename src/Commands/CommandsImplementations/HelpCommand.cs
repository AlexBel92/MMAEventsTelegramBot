using System;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApiClients;
using MMAEvents.TelegramBot.Services;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot.Commands
{
    public class HelpCommand : IDefaultCommand, IClientCommand
    {
        private readonly IEventsApiClient apiClient;
        private readonly TelegramBotService telegramService;

        public static string Name => "/help";
        public static string Description => "Что я умею?";

        public string CommandName => Name;
        public string CommandDescription => Description;

        public HelpCommand(IEventsApiClient apiClient, TelegramBotService telegramService)
        {
            this.apiClient = apiClient;
            this.telegramService = telegramService;
        }

        public async Task ExecuteAsync(Update context, CancellationToken cancellationToken = default)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var messageText = "Я умею выполнять следующие команды: \n" +
                              "/events - Получить ближайшие события \n" +
                              "/events 5 - Получить 5 ближайщих событий (максимум 9) \n" +
                              "/events UFC 123 - Поиск по названию \n" +
                              "/events 2020.11.21 - Поиск по дате \n" +
                              "/events_past - Получить прошлые события \n" +
                              "/events_past 4 - Получить 4 прошлых события (максимум 9)";

            await telegramService.SendTextMessageAsync(messageText, this.GetMessage(context), cancellationToken);
        }
    }
}