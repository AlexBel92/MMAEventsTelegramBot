using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MMAEvents.ApiClients.Models;
using MMAEvents.TelegramBot.Services;

namespace MMAEvents.TelegramBot.Controllers
{
    [ApiController]
    public class EventChangesController : ControllerBase
    {
        private readonly ILogger<EventChangesService> logger;
        private readonly TelegramBotService telegramBotService;
        private readonly IMapper mapper;

        public EventChangesController(ILogger<EventChangesService> logger, TelegramBotService telegramBotService, IMapper mapper)
        {
            this.logger = logger;
            this.telegramBotService = telegramBotService;
            this.mapper = mapper;
        }

        [HttpPost("api/EventChanges")]
        public async Task<IActionResult> EventChange(Changes eventChanges)
        {
            if (eventChanges is null)
                return BadRequest();

            var oldEventData = mapper.Map<EventDTO>(eventChanges.OldEventData);
            var newEventData = mapper.Map<EventDTO>(eventChanges.NewEventData);

            await telegramBotService.SendEventChangesAsync(TelegramBotService.EventsChannelId, oldEventData, newEventData);

            return Ok();
        }
    }
}