using Commons.Extensions;
using Domain;
using HeatingControl.Models;
using System.Collections.Generic;

namespace HeatingControl.Application.Queries
{
    public interface IHeaterSettingsProvider
    {
        HeaterSettings Provide(int heaterId, ControllerState controllerState);
    }

    public class HeaterSettings
    {
        public string Name { get; set; }
        public int PowerOutputDeviceId { get; set; }
        public string PowerOutputChannel { get; set; }
        public string PowerOutputProtocolName { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public decimal UsagePerHour { get; set; }
        public int MinimumStateChangeIntervalSeconds { get; set; }
    }

    public class HeaterSettingsProvider : IHeaterSettingsProvider
    {
        public HeaterSettings Provide(int heaterId, ControllerState controllerState)
        {
            var heater = controllerState.HeaterIdToState.GetValueOrDefault(heaterId);

            if (heater == null)
            {
                return null;
            }

            return new HeaterSettings
            {
                Name = heater.Heater.Name,
                PowerOutputDeviceId = heater.Heater.DigitalOutput.DeviceId,
                PowerOutputChannel = heater.Heater.DigitalOutput.OutputChannel,
                PowerOutputProtocolName = heater.Heater.DigitalOutput.ProtocolName,
                UsageUnit = heater.Heater.UsageUnit,
                UsagePerHour = heater.Heater.UsagePerHour,
                MinimumStateChangeIntervalSeconds = heater.Heater.MinimumStateChangeIntervalSeconds
            };
        }
    }
}
