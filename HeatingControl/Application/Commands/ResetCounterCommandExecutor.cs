using System;
using HeatingControl.Models;
using Storage.StorageDatabase.Counter;
using Commons.Extensions;
using Commons.Localization;

namespace HeatingControl.Application.Commands
{
    public class ResetCounterCommand
    {
        public int ZoneId { get; set; }
        public int UserId { get; set; }
    }

    public class ResetCounterCommandExecutor : ICommandExecutor<ResetCounterCommand>
    {
        private readonly ICounterResetter _counterResetter;
        private readonly ICounterAccumulator _counterAccumulator;

        public ResetCounterCommandExecutor(ICounterResetter counterResetter,
                                           ICounterAccumulator counterAccumulator)
        {
            _counterResetter = counterResetter;
            _counterAccumulator = counterAccumulator;
        }

        public CommandResult Execute(ResetCounterCommand command, ControllerState controllerState)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(command.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            foreach (var heaterId in zone.Zone.HeaterIds)
            {
                controllerState.HeaterIdToState[heaterId].LastCounterStart = DateTime.Now;

                _counterResetter.Reset(new CounterResetterInput
                                       {
                                           HeaterId = heaterId,
                                           UserId = command.UserId
                                       });

                _counterAccumulator.Accumulate(new CounterAccumulatorInput
                                               {
                                                   HeaterId = heaterId,
                                                   SecondsToAccumulate = 0
                                               });
            }

            return CommandResult.Empty;
        }
    }
}
