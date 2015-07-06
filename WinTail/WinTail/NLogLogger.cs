using System;
using Akka.Actor;
using Akka.Event;
using NLog;
using NLogger = global::NLog.Logger;

namespace WinTail
{
    /// <summary>
    /// This class is used to receive log events and sends them to
    /// the configured NLog logger. The following log events are
    /// recognized: <see cref="Debug"/>, <see cref="Info"/>,
    /// <see cref="Warning"/> and <see cref="Error"/>.
    /// </summary>
    public class NLogLogger : ReceiveActor
    {
        private readonly ILoggingAdapter log = Context.GetLogger();

        private static void Log(LogEvent logEvent, Action<NLogger> logStatement)
        {
            var logger = LogManager.GetLogger(logEvent.LogClass.FullName);
            logStatement(logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogLogger"/> class.
        /// </summary>
        public NLogLogger()
        {
            Receive<Error>(m => Log(m, logger => logger.ErrorException(m.Message.ToString(), m.Cause)));
            Receive<Warning>(m => Log(m, logger => logger.Warn(m.Message.ToString())));
            Receive<Info>(m => Log(m, logger => logger.Info(m.Message.ToString())));
            Receive<Debug>(m => Log(m, logger => logger.Debug(m.Message.ToString())));

            Receive<InitializeLogger>(m =>
            {
                log.Info("NLogLogger started");
                Sender.Tell(new LoggerInitialized());
            });
        }
    }
}