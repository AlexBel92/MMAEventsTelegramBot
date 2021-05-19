using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApiClients;
using MMAEvents.ApiClients.Exceptions;
using MMAEvents.TelegramBot.Services;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot.Commands
{
    public class GetPastEventsCommand : IClientCommand
    {
        private readonly IEventsApiClient apiClient;
        private readonly TelegramBotService telegramService;

        public static string Name => "/events_past";
        public static string Description => "Получить прошлые события";

        public string CommandName => Name;
        public string CommandDescription => Description;

        public GetPastEventsCommand(IEventsApiClient apiClient, TelegramBotService telegramService)
        {
            this.apiClient = apiClient;
            this.telegramService = telegramService;
        }

        public async Task ExecuteAsync(Update context, CancellationToken cancellationToken = default)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var message = this.GetMessage(context);

            try
            {
                var quantity = -2;

                if (Regex.IsMatch(message.Text, $"^{Name} [1-9]$"))
                {
                    quantity = -int.Parse(message.Text.Replace(Name + " ", string.Empty));
                }

                var events = await apiClient.GetEventsAsync(default, default, quantity, cancellationToken: cancellationToken);

                await telegramService.SendEventsAsync(message.Chat.Id, events, cancellationToken);
            }
            catch (ApiException e) when (e.StatusCode == 404)
            {
                await telegramService.SendTextMessageAsync("Ничего не найдено", message, cancellationToken);
            }
        }
    }
}