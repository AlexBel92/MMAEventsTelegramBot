using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MMAEvents.TelegramBot.Commands
{
    public interface ICommand
    {
        string CommandName { get; }
        Task ExecuteAsync(Update context, CancellationToken cancellationToken = default);
    }
}