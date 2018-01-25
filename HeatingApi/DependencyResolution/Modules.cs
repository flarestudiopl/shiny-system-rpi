using Autofac;
using HardwareAccess.Buses;
using HardwareAccess.Buses.PlatformIntegration;
using HardwareAccess.Devices;

namespace HeatingApi.DependencyResolution
{
    public class Modules
    {
        public static void Register(ContainerBuilder builder)
        {
            // PlatformIntegration
            builder.RegisterType<ProcessRunner>().As<IProcessRunner>().InstancePerLifetimeScope();
            builder.RegisterType<LibcWrapper>().As<ILibcWrapper>().InstancePerLifetimeScope();

            // Buses
            builder.RegisterType<OneWire>().As<IOneWire>().InstancePerLifetimeScope();
            builder.RegisterType<I2c>().As<II2c>().InstancePerLifetimeScope();

            // Devices
            builder.RegisterType<TemperatureSensor>().As<ITemperatureSensor>().InstancePerLifetimeScope();
        }
    }
}
