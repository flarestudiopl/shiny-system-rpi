using HeatingControl.Application.Loops.Processing;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public interface IDisableAllOutputsExecutor
    {
        void Execute(ControllerState controllerState);
    }

    public class DisableAllOutputsExecutor : IDisableAllOutputsExecutor
    {
        private readonly IOutputsWriter _outputsWriter;

        public DisableAllOutputsExecutor(IOutputsWriter outputsWriter)
        {
            _outputsWriter = outputsWriter;
        }

        public void Execute(ControllerState controllerState)
        {
            foreach (var heaterToState in controllerState.HeaterIdToState)
            {
                heaterToState.Value.OutputState = false;
            }

            _outputsWriter.Write(controllerState, true);
        }
    }
}
