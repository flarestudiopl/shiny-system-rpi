using System.Collections.Generic;
using Commons.Extensions;
using Domain.BuildingModel;
using HardwareAccess.Buses;

namespace HeatingControl.Application.Queries
{
    public interface INewHeaterOptionsProvider
    {
        NewHeaterOptionsProviderResult Provide();
    }

    public class NewHeaterOptionsProviderResult
    {
        public ICollection<int> AvailableHeaterModules { get; set; }
        public IDictionary<int, string> UsageUnits { get; set; }
    }

    public class NewHeaterOptionsProvider : INewHeaterOptionsProvider
    {
        private readonly II2c _i2C;

        public NewHeaterOptionsProvider(II2c i2C)
        {
            _i2C = i2C;
        }

        public NewHeaterOptionsProviderResult Provide()
        {
            return new NewHeaterOptionsProviderResult
                   {
                       AvailableHeaterModules = _i2C.GetI2cDevices().Result,
                       UsageUnits = EnumExtensions.AsDictionary<UsageUnit>()
                   };
        }
    }
}
