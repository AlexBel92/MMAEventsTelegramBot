using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MMAEvents.ApiClients.Models;
using MMAEvents.TelegramBot.Services;

namespace MMAEvents.TelegramBot
{
    public class EventChangesService : EventChanges.EventChangesBase
    {
        private readonly ILogger<EventChangesService> logger;
        private readonly TelegramBotService telegramBotService;
        private readonly IMapper mapper;

        public EventChangesService(ILogger<EventChangesService> logger, TelegramBotService telegramBotService, IMapper mapper)
        {
            this.logger = logger;
            this.telegramBotService = telegramBotService;
            this.mapper = mapper;
        }

        public async override Task<Empty> EventChange(Changes eventChanges, Grpc.Core.ServerCallContext context)
        {
            if (eventChanges is null)
                return new Empty();

            var oldEventData = mapper.Map<EventDTO>(eventChanges.OldEventData);
            var newEventData = mapper.Map<EventDTO>(eventChanges.NewEventData);

            await telegramBotService.SendEventChangesAsync(TelegramBotService.EventsChannelId, oldEventData, newEventData);

            return new Empty();
        }
    }
}