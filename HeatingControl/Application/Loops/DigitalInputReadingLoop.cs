using HardwareAccess.Devices;
using HeatingControl.Application.Loops.Processing;
using HeatingControl.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeatingControl.Application.Loops
{
    public interface IDigitalInputReadingLoop
    {
        void Start(ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class DigitalInputReadingLoop : IDigitalInputReadingLoop
    {
        private readonly IDigitalInputProvider _digitalInputProvider;
        private readonly IPowerSupplySignalProcessor _powerSupplySignalProcessor;

        public DigitalInputReadingLoop(IDigitalInputProvider digitalInputProvider,
                                       IPowerSupplySignalProcessor powerSupplySignalProcessor)
        {
            _digitalInputProvider = digitalInputProvider;
            _powerSupplySignalProcessor = powerSupplySignalProcessor;
        }

        public void Start(ControllerState controllerState, CancellationToken cancellationToken)
        {
            Loop.Start("Digital input processing",
                       controllerState.Model.ControlLoopIntervalSecondsMilliseconds,
                       () => ProcessReads(controllerState),
                       cancellationToken);
        }

        private void ProcessReads(ControllerState controllerState)
        {
            Parallel.ForEach(controllerState.DigitalInputFunctionToState.Values,
               async x =>
               {
                   var input = _digitalInputProvider.Provide(x.DigitalInput.ProtocolName);

                   x.State = await input.GetState(x.DigitalInput.DeviceId, x.DigitalInput.InputName);

                   if (x.DigitalInput.Inverted)
                   {
                       x.State = !x.State;
                   }

                   x.LastRead = DateTime.UtcNow;
               });

            _powerSupplySignalProcessor.Process(controllerState);
        }
    }
}
