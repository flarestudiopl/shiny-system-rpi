using System;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Application.DataAccess.Counter;

namespace HeatingControl.Application.Commands
{
    public class ResetCounterCommand
    {
        public int ZoneId { get; set; }
        public int UserId { get; set; }
    }

    public class ResetCounterCommandExecutor : ICommandExecutor<ResetCounterCommand>
    {
        private readonly IRepository<Counter> _counterRepository;
        private readonly ICounterAccumulator _counterAccumulator;

        public ResetCounterCommandExecutor(IRepository<Counter> counterRepository,
                                           ICounterAccumulator counterAccumulator)
        {
            _counterRepository = counterRepository;
            _counterAccumulator = counterAccumulator;
        }

        public CommandResult Execute(ResetCounterCommand command, CommandContext context)
        {
            var zone = context.ControllerState.ZoneIdToState.GetValueOrDefault(command.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            foreach (var heater in zone.Zone.Heaters)
            {
                context.ControllerState.HeaterIdToState[heater.HeaterId].LastCounterStart = DateTime.UtcNow;

                var counter = _counterRepository.ReadSingleOrDefault(x => x.HeaterId == heater.HeaterId && !x.ResetDate.HasValue);

                counter.ResetDate = DateTime.UtcNow;
                counter.ResettedByUserId = command.UserId;

                _counterRepository.Update(counter);

                _counterAccumulator.Accumulate(new CounterAccumulatorInput
                {
                    HeaterId = heater.HeaterId,
                    SecondsToAccumulate = 0
                });
            }

            return CommandResult.Empty;
        }
    }
}
