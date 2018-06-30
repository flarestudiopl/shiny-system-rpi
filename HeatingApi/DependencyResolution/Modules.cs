using Autofac;
using HardwareAccess.Buses.PlatformIntegration;
using HardwareAccess.Buses;
using HardwareAccess.Devices;
using HeatingControl.Application;
using HeatingControl.Application.Loops;
using HeatingControl.Application.Loops.Processing;
using Microsoft.Extensions.Hosting;
using Storage.StorageDatabase;

namespace HeatingApi.DependencyResolution
{
    public class Modules
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterPersistence(builder);

#if DEBUG
            RegisterDummyHardwareAccess(builder);
#else
            RegisterHardwareAccess(builder);
#endif
            RegisterControl(builder);
        }

        private static void RegisterPersistence(ContainerBuilder builder)
        {
            var assembly = typeof(ISqlConnectionResolver).Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }

        private static void RegisterHardwareAccess(ContainerBuilder builder)
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
        }

        private static void RegisterDummyHardwareAccess(ContainerBuilder builder)
        {
            // Buses
            builder.RegisterType<OneWire>().As<IOneWire>().SingleInstance();
            builder.RegisterType<HardwareAccess.Dummy.Buses.I2c>().As<II2c>().SingleInstance();

            // Devices
            builder.RegisterType<HardwareAccess.Dummy.Devices.PowerOutput>().As<IPowerOutput>().SingleInstance();
            builder.RegisterType<HardwareAccess.Dummy.Devices.TemperatureSensor>().As<ITemperatureSensor>().SingleInstance();
        }

        private static void RegisterControl(ContainerBuilder builder)
        {
            var assembly = typeof(IControllerStateBuilder).Assembly;

            builder.RegisterType<CommandHandler>().As<ICommandHandler>().SingleInstance();

            builder.RegisterType<HeatingControl>()
                   .As<IHeatingControl>()
                   .As<IHostedService>()
                   .SingleInstance();

            // Application
            builder.RegisterType<ControllerStateBuilder>().As<IControllerStateBuilder>().SingleInstance();

            // Application/Commands
            builder.RegisterAssemblyTypes(assembly)
                   .Where(x => x.FullName.StartsWith("HeatingControl.Application.Commands."))
                   .AsImplementedInterfaces()
                   .SingleInstance();

            // Application/Loops
            builder.RegisterType<OutputStateProcessingLoop>().As<IOutputStateProcessingLoop>().SingleInstance();
            builder.RegisterType<ScheduleDeterminationLoop>().As<IScheduleDeterminationLoop>().SingleInstance();
            builder.RegisterType<TemperatureReadingLoop>().As<ITemperatureReadingLoop>().SingleInstance();

            // Application/Loops/Processing
            builder.RegisterType<HysteresisProcessor>().As<IHysteresisProcessor>().SingleInstance();
            builder.RegisterType<OutputsWriter>().As<IOutputsWriter>().SingleInstance();
            builder.RegisterType<PowerZoneOutputLimiter>().As<IPowerZoneOutputLimiter>().SingleInstance();
            builder.RegisterType<UsageCollector>().As<IUsageCollector>().SingleInstance();
            builder.RegisterType<ZonePowerProvider>().As<IZonePowerProvider>().SingleInstance();
            builder.RegisterType<ZoneTemperatureProvider>().As<IZoneTemperatureProvider>().SingleInstance();

            // Application/Queries
            builder.RegisterAssemblyTypes(assembly)
                   .Where(x => x.FullName.StartsWith("HeatingControl.Application.Queries."))
                   .AsImplementedInterfaces()
                   .SingleInstance();
        }
    }
}
