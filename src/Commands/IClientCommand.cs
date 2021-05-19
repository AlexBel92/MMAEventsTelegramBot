namespace MMAEvents.TelegramBot.Commands
{
    public interface IClientCommand : ICommand
    {
        string CommandDescription { get; }
    }
}