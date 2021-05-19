using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MMAEvents.TelegramBot.Commands;
using Microsoft.Extensions.Logging;
using System;

namespace MMAEvents.TelegramBot.Controllers
{
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly CommandsCollection commands;
        private readonly ILogger<WebHookController> logger;

        public WebHookController(CommandsCollection commands, ILogger<WebHookController> logger)
        {
            this.commands = commands;
            this.logger = logger;
        }

        [HttpPost("api/web-hook")]
        public async Task<IActionResult> WebHook(Update update)
        {
            var commandName = GetCommandName(update);
            var command = commands.GetCommandOrDefault(commandName);

            try
            {
                if (command is not null)
                    await command.ExecuteAsync(update);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"CommandName: {commandName}, Command: {command ?? null}");
            }

            return Ok();
        }

        private string GetCommandName(Update update)
        {
            var commandName = string.Empty;

            if (update.CallbackQuery is not null && update.CallbackQuery.Data.StartsWith("/"))
                commandName = update.CallbackQuery.Data;
            else if (update.Message is not null && update.Message.Text.StartsWith("/"))
                commandName = update.Message.Text;

            if (commandName is not null)
            {
                var spaceIndex = commandName.IndexOf(" ");
                if (spaceIndex > 0)
                    commandName = commandName.Substring(0, spaceIndex);
            }

            return commandName;
        }
    }
}