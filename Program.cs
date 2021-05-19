using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMAEvents.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MMAEvents.TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                try
                {
                    var telegramBotClient = serviceProvider.GetRequiredService<TelegramBotClient>();
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var webHookUri = configuration.GetSection("TelegramBot_HookUri").Value;

                    if (!Uri.TryCreate(webHookUri, UriKind.Absolute, out _))
                        throw new UriFormatException(webHookUri + " is not absolute uri string");

                    telegramBotClient.SetMyCommandsAsync(serviceProvider.GetRequiredService<CommandsCollection>().GetClientCommands()).Wait();
                    telegramBotClient.SetWebhookAsync(webHookUri, allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery }).Wait();
                    logger.LogInformation(new EventId(), "Set web hook: " + webHookUri);
                }
                catch (Exception e)
                {
                    logger.LogCritical(new EventId(), e, "Error occurred while starting app");
                    throw;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
