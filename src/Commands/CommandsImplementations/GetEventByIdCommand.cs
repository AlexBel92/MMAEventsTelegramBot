using System;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApiClients;
using MMAEvents.ApiClients.Exceptions;
using MMAEvents.TelegramBot.Services;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot.Commands
{
    public class GetEventByIdCommand : ICommand
    {
        private readonly IEventsApiClient apiClient;
        private readonly TelegramBotService telegramService;

        public static string Name => "/get_event_by_id";
        public string CommandName => Name;

        public GetEventByIdCommand(IEventsApiClient apiClient, TelegramBotService telegramService)
        {
            this.apiClient = apiClient;
            this.telegramService = telegramService;
        }

        public async Task ExecuteAsync(Update context, CancellationToken cancellationToken = default)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.CallbackQuery is null)
                return;

            var message = this.GetMessage(context);
            message.Chat.Id = context.CallbackQuery.From.Id;

            try
            {
                var id = long.Parse(context.CallbackQuery.Data.Replace(Name, string.Empty));

                var eventDTO = await apiClient.GetEventByIdAsync(id);

                await telegramService.SendSingleEventAsync(message.Chat.Id, eventDTO, cancellationToken);
            }
            catch (ApiException e) when (e.StatusCode == 404)
            {                
                await telegramService.SendTextMessageAsync("Ничего не найдено", message, cancellationToken);
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                throw new CommandFormatExceptions($"Command: {context.CallbackQuery.Data} was in incorrect format", e);
            }
        }
    }
}