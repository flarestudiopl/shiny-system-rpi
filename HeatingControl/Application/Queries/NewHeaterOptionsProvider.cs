using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HardwareAccess.Devices;

namespace HeatingControl.Application.Queries
{
    public interface INewHeaterOptionsProvider
    {
        NewHeaterOptionsProviderResult Provide();
    }

    public class NewHeaterOptionsProviderResult
    {
        public ICollection<SupportedOutputProtocol> AvailableHeaterModules { get; set; }
        public IDictionary<int, string> UsageUnits { get; set; }

        public class SupportedOutputProtocol
        {
            public string ProtocolName { get; set; }
            public object ConfigurationOptions { get; set; }
        }
    }

    public class NewHeaterOptionsProvider : INewHeaterOptionsProvider
    {
        private readonly IPowerOutputProvider _powerOutputProvider;

        public NewHeaterOptionsProvider(IPowerOutputProvider powerOutputProvider)
        {
            _powerOutputProvider = powerOutputProvider;
        }

        public NewHeaterOptionsProviderResult Provide()
        {
            var availableHeaterModules = _powerOutputProvider.GetAvailableProtocolNames()
                                                             .Select(x =>
                                                             {
                                                                 var powerOutput = _powerOutputProvider.Provide(x);
                                                             
                                                                 return new NewHeaterOptionsProviderResult.SupportedOutputProtocol
                                                                 {
                                                                     ProtocolName = x,
                                                                     ConfigurationOptions = powerOutput.ConfigurationOptions
                                                                 };
                                                             })
                                                             .ToList();

            return new NewHeaterOptionsProviderResult
            {
                AvailableHeaterModules = availableHeaterModules,
                UsageUnits = EnumExtensions.AsDictionary<UsageUnit>()
            };
        }
    }
}
