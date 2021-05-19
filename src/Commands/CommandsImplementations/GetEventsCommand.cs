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
    public class GetEventsCommand : IClientCommand
    {
        private readonly IEventsApiClient apiClient;
        private readonly TelegramBotService telegramService;

        public static string Name => "/events";
        public static string Description => "Получить ближайшие события";

        public string CommandName => Name;
        public string CommandDescription => Description;

        public GetEventsCommand(IEventsApiClient apiClient, TelegramBotService telegramService)
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
                var filterData = GetDataForFilter(message);

                var events = await apiClient.GetEventsAsync(filterData.EventName, filterData.Date, filterData.Quantity, cancellationToken: cancellationToken);

                await telegramService.SendEventsAsync(message.Chat.Id, events, cancellationToken);
            }
            catch (ApiException e) when (e.StatusCode == 404)
            {
                await telegramService.SendTextMessageAsync("Ничего не найдено", message, cancellationToken);
            }
        }

        private static FilterData GetDataForFilter(Message message)
        {
            var filterData = new FilterData();

            if (Regex.IsMatch(message.Text, $"^{Name} [1-9]$", RegexOptions.None))
            {
                filterData.Quantity = int.Parse(message.Text.Replace(Name + " ", string.Empty));
            }
            else if (DateTimeOffset.TryParse(message.Text.Replace(Name + " ", string.Empty), out DateTimeOffset dateParse))
            {
                filterData.Date = dateParse;
            }
            else if (Regex.IsMatch(message.Text, $"^{Name} .+$", RegexOptions.None))
            {
                var eventName = message.Text.Replace(Name + " ", string.Empty);
                filterData.EventName = eventName.Substring(0, Math.Min(eventName.Length, 15));
            }

            return filterData;
        }

        private class FilterData
        {
            public string EventName { get; set; }
            public int? Quantity { get; set; }
            public DateTimeOffset? Date { get; set; }
        }
    }
}