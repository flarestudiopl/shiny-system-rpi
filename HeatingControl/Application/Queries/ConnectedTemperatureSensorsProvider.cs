using System.Collections.Generic;
using System.Linq;
using Domain;
using HardwareAccess.Devices;

namespace HeatingControl.Application.Queries
{
    public interface IConnectedTemperatureSensorsProvider
    {
        ICollection<AvailableTemperatureInputProtocol> Provide(Building model);
    }

    public class AvailableTemperatureInputProtocol
    {
        public string ProtocolName { get; set; }
        public object ConfigurationOptions { get; set; }
    }

    public class ConnectedTemperatureSensorsProvider : IConnectedTemperatureSensorsProvider
    {
        private readonly ITemperatureInputProvider _temperatureInputProvider;

        public ConnectedTemperatureSensorsProvider(ITemperatureInputProvider temperatureInputProvider)
        {
            _temperatureInputProvider = temperatureInputProvider;
        }

        public ICollection<AvailableTemperatureInputProtocol> Provide(Building model)
        {
            return _temperatureInputProvider.GetAvailableProtocolNames()
                                            .Select(x =>
                                                {
                                                    return new AvailableTemperatureInputProtocol
                                                    {
                                                        ProtocolName = x,
                                                        ConfigurationOptions = _temperatureInputProvider.Provide(x).ConfigurationOptions
                                                    };
                                                })
                                            .ToList();
        }
    }
}
