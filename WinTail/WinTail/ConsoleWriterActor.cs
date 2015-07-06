using System;
using Akka.Actor;
using Akka.Event;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            if (message is Messages.InputError)
            {
                var msg = message as Messages.InputError;
                log.Error("Messages.InputError seen : " + msg.Reason);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Reason);
            }
            else if (message is Messages.InputSuccess)
            {
                var msg = message as Messages.InputSuccess;
                log.Info("Messages.InputSuccess seen : " + msg.Reason);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg.Reason);
            }
            else
            {
                log.Info(message.ToString());
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }
    }
}