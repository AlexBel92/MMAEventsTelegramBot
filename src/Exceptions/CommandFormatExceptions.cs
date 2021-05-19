using System;
using System.Runtime.Serialization;

namespace MMAEvents.TelegramBot
{
    [Serializable]
    public class CommandFormatExceptions : Exception
    {
        public CommandFormatExceptions() { }
        public CommandFormatExceptions(string message) : base(message) { }
        public CommandFormatExceptions(string message, System.Exception inner) : base(message, inner) { }
        protected CommandFormatExceptions(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}