using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApiClients.Models;
using MMAEvents.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MMAEvents.TelegramBot.Services
{
    public class TelegramBotService
    {
        private readonly TelegramBotClient telegramBotClient;

        public static long EventsChannelId => -1001236535566;

        public TelegramBotService(TelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        public async Task SendTextMessageAsync(string messageText, Message initMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                throw new ArgumentException(nameof(messageText));

            if (initMessage is null)
                throw new ArgumentNullException(nameof(initMessage));

            await telegramBotClient.SendTextMessageAsync(initMessage.Chat.Id, messageText, replyToMessageId: initMessage.MessageId, cancellationToken: cancellationToken);
        }

        public async Task SendEventsAsync(long chatId, ICollection<EventDTO> events, CancellationToken cancellationToken = default)
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            if (events.Count == 1)
            {
                await SendSingleEventAsync(chatId, events.First(), cancellationToken);
                return;
            }

            var messageBuilder = new StringBuilder();
            var buttons = new List<InlineKeyboardButton>();
            var count = 0;


            foreach (var eventDTO in events)
            {
                messageBuilder.Append("● ");
                messageBuilder.Append(eventDTO.Name);
                messageBuilder.Append(" ");
                messageBuilder.Append(GetEventDateString(eventDTO));
                messageBuilder.Append("\n");

                buttons.Add(new InlineKeyboardButton()
                {
                    Text = eventDTO.Name,
                    CallbackData = GetEventByIdCommand.Name + " " + eventDTO.Id
                });

                count++;
                if (count == 4)
                {
                    await telegramBotClient.SendTextMessageAsync(
                                                chatId,
                                                messageBuilder.ToString(),
                                                replyMarkup: new InlineKeyboardMarkup(buttons),
                                                cancellationToken: cancellationToken);
                    count = 0;
                    messageBuilder = messageBuilder.Clear();
                    buttons.Clear();
                }
            }

            if (messageBuilder.Length != 0)
                await telegramBotClient.SendTextMessageAsync(
                                            chatId,
                                            messageBuilder.ToString(),
                                            replyMarkup: new InlineKeyboardMarkup(buttons),
                                            cancellationToken: cancellationToken);
        }

        public async Task SendSingleEventAsync(long chatId, EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            if (eventDTO is null)
                throw new ArgumentNullException(nameof(eventDTO));

            var messageBuilder = new StringBuilder();
            var buttons = new List<InlineKeyboardButton>();

            AppendEventName(eventDTO, messageBuilder);
            messageBuilder.Append(GetEventDateString(eventDTO));
            messageBuilder.Append(" ");
            messageBuilder.Append(eventDTO.Location);
            messageBuilder.Append("\n");
            messageBuilder.Append("\n");

            if (eventDTO.FightCard != null)
                foreach (var card in eventDTO.FightCard)
                {
                    AppendCardName(messageBuilder, card);
                    foreach (var fight in card.Fights)
                    {
                        AppendFightShortInfo(messageBuilder, fight);
                    }
                    messageBuilder.Append("\n");
                }

            if (Uri.IsWellFormedUriString(eventDTO.ImgSrc, UriKind.RelativeOrAbsolute))
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Показать обложку",
                    CallbackData = GetEventImgCommand.Name + " " + eventDTO.Id
                });
            }

            if (eventDTO.IsScheduled == false)
            {
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Показать результаты боев",
                    CallbackData = GetEventResultsByIdCommand.Name + " " + eventDTO.Id
                });
            }

            await telegramBotClient.SendTextMessageAsync(
                                        chatId, messageBuilder.ToString(),
                                        parseMode: ParseMode.Html,
                                        replyMarkup: new InlineKeyboardMarkup(buttons),
                                        cancellationToken: cancellationToken);
        }

        public async Task SendEventResultsAsync(Message message, EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            if (eventDTO is null)
                throw new ArgumentNullException(nameof(eventDTO));

            var messageBuilder = new StringBuilder();
            var buttons = new List<InlineKeyboardButton>();

            AppendEventName(eventDTO, messageBuilder);
            messageBuilder.Append(GetEventDateString(eventDTO));
            messageBuilder.Append(" ");
            messageBuilder.Append(eventDTO.Location);
            messageBuilder.Append("\n");
            messageBuilder.Append("\n");

            if (eventDTO.FightCard != null)
                foreach (var card in eventDTO.FightCard)
                {
                    AppendCardName(messageBuilder, card);
                    foreach (var fight in card.Fights)
                    {
                        AppendFightFullInfo(messageBuilder, fight);
                    }
                    messageBuilder.Append("\n");
                }

            if (eventDTO.BonusAwards != null && eventDTO.BonusAwards.Any())
            {
                messageBuilder.Append("<u>Бонусные награды:</u>\n");
                foreach (var award in eventDTO.BonusAwards)
                    messageBuilder.Append("- " + award + "\n");
            }

            var replyMarkup = message.ReplyMarkup.InlineKeyboard.SelectMany(e => e)
                .Where(button => !button.CallbackData.StartsWith(GetEventResultsByIdCommand.Name));
            await telegramBotClient.EditMessageTextAsync(
                                        message.Chat.Id,
                                        message.MessageId,
                                        messageBuilder.ToString(),
                                        parseMode: ParseMode.Html,
                                        replyMarkup: new InlineKeyboardMarkup(replyMarkup),
                                        cancellationToken: cancellationToken);
        }

        public async Task SendEventImgAsync(Message message, EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            if (eventDTO is null)
                throw new ArgumentNullException(nameof(eventDTO));

            if (!Uri.IsWellFormedUriString(eventDTO.ImgSrc, UriKind.RelativeOrAbsolute))
                throw new UriFormatException(nameof(eventDTO.ImgSrc));

            var uri = new Uri(eventDTO.ImgSrc);

            await telegramBotClient.SendPhotoAsync(
                                        message.Chat.Id,
                                        uri.Host + uri.PathAndQuery,
                                        replyToMessageId: message.MessageId,
                                        cancellationToken: cancellationToken);

            var replyMarkup = message.ReplyMarkup.InlineKeyboard.SelectMany(e => e)
                .Where(button => !button.CallbackData.StartsWith(GetEventImgCommand.Name));

            await telegramBotClient.EditMessageReplyMarkupAsync(
                                        message.Chat.Id,
                                        message.MessageId,
                                        new InlineKeyboardMarkup(replyMarkup),
                                        cancellationToken);
        }

        public async Task SendEventChangesAsync(long chatId, EventDTO oldEventData, EventDTO newEventData, CancellationToken cancellationToken = default)
        {
            if (newEventData != null && newEventData.Date - DateTime.Now >= TimeSpan.FromDays(60))
                return;

            var button = new InlineKeyboardButton();
            button.Text = "Подробнее о турнире";
            button.CallbackData = GetEventByIdCommand.Name + " " + newEventData.Id;
            var replyMarkup = new InlineKeyboardMarkup(button);
            var messageBuilder = new StringBuilder();

            if (oldEventData is null && newEventData is not null)
            {
                messageBuilder.Append(
                    $"Запланирован новый турнир {newEventData.Name} {GetEventDateString(newEventData)}");
            }
            else if (oldEventData is not null && newEventData is not null)
            {
                if (oldEventData.IsScheduled == false && newEventData.IsScheduled == false)
                    return;

                if (newEventData.IsCanceled == true && oldEventData.IsCanceled == false)
                {
                    replyMarkup = null;
                    messageBuilder.Append(
                        $"Турнир {newEventData.Name} {GetEventDateString(newEventData)} был отменен");
                }
                else if (newEventData.IsScheduled == false && oldEventData.IsScheduled == true)
                {
                    messageBuilder.Append(
                        $"Прошел турнир {newEventData.Name} {GetEventDateString(newEventData)}");
                }
                else if (newEventData.Name != oldEventData.Name)
                {
                    messageBuilder.Append(
                        $"Название турнира <s>{oldEventData.Name}</s> изменилось на <b>{newEventData.Name}</b>");
                }
                else if (newEventData.Date != oldEventData.Date)
                {
                    messageBuilder.Append(
                        $"Турнир {newEventData.Name} перенесен с <s>{GetEventDateString(oldEventData)}</s> на <b>{GetEventDateString(newEventData)}</b>");
                }
                else if (oldEventData.FightCard is not null || newEventData.FightCard is not null)
                {
                    var fightComparer = new FightComparer();

                    if (CardsDiffer(oldEventData.FightCard, newEventData.FightCard, fightComparer))
                    {
                        var oldFights = new List<FightDTO>();

                        messageBuilder.Append(
                            $"Изменение карда турнира {newEventData.Name} {GetEventDateString(newEventData)} \n");

                        if (oldEventData.FightCard is not null)
                            oldFights.AddRange(oldEventData.FightCard.SelectMany(cards => cards.Fights.ToList()));

                        foreach (var card in newEventData.FightCard)
                        {
                            AppendCardName(messageBuilder, card);
                            foreach (var fight in card.Fights)
                            {
                                if (oldFights.Contains(fight, fightComparer))
                                {
                                    oldFights.RemoveAll(f => fightComparer.Equals(f, fight));

                                    AppendFightShortInfo(messageBuilder, fight);
                                }
                                else
                                {
                                    messageBuilder.Append("<b>");
                                    AppendFightShortInfo(messageBuilder, fight);
                                    messageBuilder.Append("</b>");
                                }
                            }
                        }

                        if (oldFights.Any())
                        {
                            messageBuilder.Append("<u>Отмененные бои</u>");
                            messageBuilder.Append("\n");
                            foreach (var fight in oldFights)
                            {
                                messageBuilder.Append("<s>");
                                AppendFightShortInfo(messageBuilder, fight);
                                messageBuilder.Append("</s>");
                            }
                        }
                    }
                }
            }

            if (messageBuilder.Length > 0)
                await telegramBotClient.SendTextMessageAsync(
                                            chatId,
                                            messageBuilder.ToString(),
                                            parseMode: ParseMode.Html,
                                            replyMarkup: replyMarkup,
                                            cancellationToken: cancellationToken);
        }

        private static string GetEventDateString(EventDTO eventDTO)
        {
            return eventDTO.Date.Date.ToString("dd/MM/yyyy");
        }

        private static void AppendCardName(StringBuilder messageBuilder, FightCardDTO card)
        {
            messageBuilder.Append("<u>" + card.Name + "</u>");
            messageBuilder.Append("\n");
        }

        private static void AppendEventName(EventDTO eventDTO, StringBuilder messageBuilder)
        {
            messageBuilder.Append("<b>" + eventDTO.Name + "</b>");
            messageBuilder.Append("\n");
        }

        private static void AppendFightShortInfo(StringBuilder messageBuilder, FightDTO fight)
        {
            (var firtsFighter, var secondFighter) = SwapFighters(fight.FirtsFighter, fight.SecondFighter);

            messageBuilder.Append("● ");
            messageBuilder.Append(firtsFighter);
            messageBuilder.Append(" <i>vs.</i> ");
            messageBuilder.Append(secondFighter);
            messageBuilder.Append($" ({fight.WeightClass})");
            messageBuilder.Append("\n");
        }

        private static void AppendFightFullInfo(StringBuilder messageBuilder, FightDTO fight)
        {
            messageBuilder.Append("● ");
            messageBuilder.Append(fight.FirtsFighter);
            messageBuilder.Append(" <i>def.</i> ");
            messageBuilder.Append(fight.SecondFighter);
            messageBuilder.Append($" ({fight.WeightClass})");
            messageBuilder.Append("\n");
            messageBuilder.Append("\t");
            messageBuilder.Append("○ ");
            messageBuilder.Append($" {fight.Method} {fight.Round} {fight.Time}");
            messageBuilder.Append("\n");
        }

        private static (string, string) SwapFighters(string firtsFighter, string secondFighter)
        {
            if (string.Compare(firtsFighter, secondFighter) > 0)
                (firtsFighter, secondFighter) = (secondFighter, firtsFighter);

            return (firtsFighter, secondFighter);
        }

        private bool CardsDiffer(IEnumerable<FightCardDTO> a, IEnumerable<FightCardDTO> b, IEqualityComparer<FightDTO> comparer)
        {
            if (a == null ^ b == null)
                return true;

            return !a.SelectMany(f => f.Fights).SequenceEqual(b.SelectMany(f => f.Fights), comparer);
        }
    }
}

class FightComparer : IEqualityComparer<FightDTO>
{
    public bool Equals(FightDTO x, FightDTO y)
    {
        if (Object.ReferenceEquals(x, y)) return true;

        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

        return (x.FirtsFighter == y.FirtsFighter && x.SecondFighter == y.SecondFighter) ||
               (x.FirtsFighter == y.SecondFighter && x.SecondFighter == y.FirtsFighter);
    }

    public int GetHashCode([DisallowNull] FightDTO obj)
    {
        return HashCode.Combine(obj.FirtsFighter, obj.SecondFighter, obj.WeightClass);
    }
}