using Autofac;
using HardwareAccess.Buses;
using HardwareAccess.Buses.PlatformIntegration;
using HardwareAccess.Devices;
using HeatingControl;

namespace HeatingApi.DependencyResolution
{
    public class Modules
    {
        public static void Register(ContainerBuilder builder)
        {
            // PlatformIntegration
            builder.RegisterType<ProcessRunner>().As<IProcessRunner>().SingleInstance();
            builder.RegisterType<LibcWrapper>().As<ILibcWrapper>().SingleInstance();

            // Buses
            builder.RegisterType<OneWire>().As<IOneWire>().SingleInstance();
            builder.RegisterType<I2c>().As<II2c>().SingleInstance();

            // Devices
            builder.RegisterType<PowerOutput>().As<IPowerOutput>().SingleInstance();
            builder.RegisterType<TemperatureSensor>().As<ITemperatureSensor>().SingleInstance();

            // HeatingControl
            builder.RegisterType<BuildingModelProvider>().As<IBuildingModelProvider>().SingleInstance();
            builder.RegisterType<ControllerStateBuilder>().As<IControllerStateBuilder>().SingleInstance();
            builder.RegisterType<HeatingControl.HeatingControl>().As<IHeatingControl>().SingleInstance();
            builder.RegisterType<TemperatureReadingLoop>().As<ITemperatureReadingLoop>().SingleInstance();
        }
    }
}
