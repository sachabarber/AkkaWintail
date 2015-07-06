using System.IO;
using Akka.Actor;
using Akka.Event;

namespace WinTail
{
    /// <summary>
    /// Actor that validates user input and signals result to others.
    /// </summary>
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;
        private readonly ILoggingAdapter log = Context.GetLogger();

        public FileValidatorActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                log.Error("Empty filename");

                // signal that the user needs to supply an input
                _consoleWriterActor.Tell(new Messages.NullInputError("Input was blank. Please try again.\n"));

                // tell sender to continue doing its thing (whatever that may be, this actor doesn't care)
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    log.Info("File is valid");
                    // signal successful input
                    _consoleWriterActor.Tell(new Messages.InputSuccess(string.Format("Starting processing for {0}", msg)));

                    // start coordinator
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                }
                else
                {
                    log.Error("File is invalid");
                    
                    // signal that input was bad
                    _consoleWriterActor.Tell(new Messages.ValidationError(string.Format("{0} is not an existing URI on disk.", msg)));

                    // tell sender to continue doing its thing (whatever that may be, this actor doesn't care)
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }



        }

        /// <summary>
        /// Checks if file exists at path provided by user.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}