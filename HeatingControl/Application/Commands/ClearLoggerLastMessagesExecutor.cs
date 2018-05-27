using Commons;

namespace HeatingControl.Application.Commands
{
    public interface IClearLoggerLastMessagesExecutor
    {
        void Execute();
    }

    public class ClearLoggerLastMessagesExecutor : IClearLoggerLastMessagesExecutor
    {
        public void Execute()
        {
            Logger.LastMessages.Clear();
        }
    }
}
