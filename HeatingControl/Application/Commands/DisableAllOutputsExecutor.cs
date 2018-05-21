using System.Linq;
using Domain.BuildingModel;
using HardwareAccess.Buses;

namespace HeatingControl.Application.Commands
{
    public interface IDisableAllOutputsExecutor
    {
        void Execute(Building building);
    }

    public class DisableAllOutputsExecutor : IDisableAllOutputsExecutor
    {
        private readonly II2c _i2c;

        public DisableAllOutputsExecutor(II2c i2c)
        {
            _i2c = i2c;
        }

        public void Execute(Building building)
        {
            foreach (var device in building.Heaters.Select(x => x.PowerOutputDeviceId))
            {
                _i2c.WriteToDevice(device, byte.MaxValue);
            }
        }
    }
}
