using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.Extensions;
using Domain;
using HardwareAccess.Devices;

namespace HeatingControl.Application.Queries
{
    public interface INewHeaterOptionsProvider
    {
        Task<NewHeaterOptionsProviderResult> Provide();
    }

    public class NewHeaterOptionsProviderResult
    {
        public ICollection<SupportedOutputProtocol> AvailableHeaterModules { get; set; }
        public IDictionary<int, string> UsageUnits { get; set; }

        public class SupportedOutputProtocol
        {
            public string ProtocolName { get; set; }
            public ICollection<string> OutputNames { get; set; }
            public ICollection<int> AvailableDeviceIds { get; set; }
        }
    }

    public class NewHeaterOptionsProvider : INewHeaterOptionsProvider
    {
        private readonly IPowerOutputProvider _powerOutputProvider;

        public NewHeaterOptionsProvider(IPowerOutputProvider powerOutputProvider)
        {
            _powerOutputProvider = powerOutputProvider;
        }

        public async Task<NewHeaterOptionsProviderResult> Provide()
        {
            var availableHeaterModules = await Task.WhenAll(_powerOutputProvider.GetAvailableProtocolNames()
                                                                                .Select(async x =>
                                                                                {
                                                                                    var powerOutput = _powerOutputProvider.Provide(x);
                                                                                
                                                                                    return new NewHeaterOptionsProviderResult.SupportedOutputProtocol
                                                                                    {
                                                                                        ProtocolName = x,
                                                                                        OutputNames = powerOutput.OutputNames,
                                                                                        AvailableDeviceIds = await powerOutput.GetDeviceIds()
                                                                                    };
                                                                                }));

            return new NewHeaterOptionsProviderResult
            {
                AvailableHeaterModules = availableHeaterModules,
                UsageUnits = EnumExtensions.AsDictionary<UsageUnit>()
            };
        }
    }
}
