using System.Collections.Generic;
using HeatingControl.Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public interface IZoneControlModeExecutor
    {
        void Execute(ZoneControlModeExecutorInput input, ControllerState state);
    }

    public class ZoneControlModeExecutorInput
    {
        public int ZoneId { get; set; }
        public ZoneControlMode ControlMode { get; set; }
    }


    public class ZoneControlModeExecutor : IZoneControlModeExecutor
    {
        public void Execute(ZoneControlModeExecutorInput input, ControllerState state)
        {
            var zone = state.ZoneIdToState.GetValueOrDefault(input.ZoneId);

            if (zone == null)
            {
                return;
            }

            zone.ControlMode = input.ControlMode;
        }
    }
}
