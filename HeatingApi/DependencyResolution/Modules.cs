using Autofac;
using HardwareAccess.PlatformIntegration;
using HardwareAccess.Buses;
using HardwareAccess.Devices;
using HeatingControl.Application;
using HeatingControl.Application.Loops;
using HeatingControl.Application.Loops.Processing;
using Microsoft.Extensions.Hosting;
using Storage.StorageDatabase;
using HeatingControl.Application.DataAccess;
using HardwareAccess.Devices.PowerOutputs;
using HardwareAccess.Devices.DigitalInputs;
using HardwareAccess.Devices.TemperatureInputs;

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
            var assembly = typeof(IDbExecutor).Assembly;

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
            builder.RegisterType<ModbusTcp>().As<IModbusTcp>().SingleInstance();
            builder.RegisterType<I2c>().As<II2c>().SingleInstance();

            // Devices
            builder.RegisterType<PowerOutputProvider>().As<IPowerOutputProvider>().SingleInstance();
            builder.RegisterType<DigitalInputProvider>().As<IDigitalInputProvider>().SingleInstance();
            builder.RegisterType<TemperatureInputProvider>().As<ITemperatureInputProvider>().SingleInstance();

            // Devices/DigitalInputs
            builder.RegisterType<ShinyPowerState>().As<IShinyPowerState>().SingleInstance();

            // Devices/PowerOutputs
            builder.RegisterType<InvertedPcfOutput>().As<IInvertedPcfOutput>().SingleInstance();
            builder.RegisterType<ShinyMcpExpander>().As<IShinyMcpExpander>().SingleInstance();
            builder.RegisterType<HardwareAccess.Devices.PowerOutputs.FlowairTBox>().As<HardwareAccess.Devices.PowerOutputs.IFlowairTBox>().SingleInstance();

            // Devices/TemperatureInputs
            builder.RegisterType<Ds1820>().As<IDs1820>().SingleInstance();
            builder.RegisterType<HardwareAccess.Devices.TemperatureInputs.FlowairTBox>().As<HardwareAccess.Devices.TemperatureInputs.IFlowairTBox>().SingleInstance();
        }

        private static void RegisterDummyHardwareAccess(ContainerBuilder builder)
        {
            // PlatformIntegration
            builder.RegisterType<ProcessRunner>().As<IProcessRunner>().SingleInstance();
            builder.RegisterType<HardwareAccess.Dummy.PlatformIntegration.LibcWrapper >().As<ILibcWrapper>().SingleInstance();

            // Buses
            builder.RegisterType<OneWire>().As<IOneWire>().SingleInstance();
            builder.RegisterType<HardwareAccess.Dummy.Buses.ModbusTcp>().As<IModbusTcp>().SingleInstance();
            builder.RegisterType<HardwareAccess.Dummy.Buses.I2c>().As<II2c>().SingleInstance();

            // Devices
            builder.RegisterType<PowerOutputProvider>().As<IPowerOutputProvider>().SingleInstance();
            builder.RegisterType<DigitalInputProvider>().As<IDigitalInputProvider>().SingleInstance();
            builder.RegisterType<TemperatureInputProvider>().As<ITemperatureInputProvider>().SingleInstance();

            // Devices/DigitalInputs
            builder.RegisterType<HardwareAccess.Dummy.Devices.DigitalInputs.ShinyPowerState>().As<IShinyPowerState>().SingleInstance();

            // Devices/PowerOutputs
            builder.RegisterType<InvertedPcfOutput>().As<IInvertedPcfOutput>().SingleInstance();
            builder.RegisterType<ShinyMcpExpander>().As<IShinyMcpExpander>().SingleInstance();
            builder.RegisterType<HardwareAccess.Devices.PowerOutputs.FlowairTBox>().As<HardwareAccess.Devices.PowerOutputs.IFlowairTBox>().SingleInstance();

            // Devices/TemperatureInputs
            builder.RegisterType<HardwareAccess.Dummy.Devices.TemperatureInputs.Ds1820>().As<IDs1820>().SingleInstance();
            builder.RegisterType<HardwareAccess.Devices.TemperatureInputs.FlowairTBox>().As<HardwareAccess.Devices.TemperatureInputs.IFlowairTBox>().SingleInstance();
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

            // Application/DataAccess
            builder.RegisterGeneric(typeof(Repository<>))
                   .As(typeof(IRepository<>));

            builder.RegisterAssemblyTypes(assembly)
                   .Where(x => x.FullName.StartsWith("HeatingControl.Application.DataAccess."))
                   .AsImplementedInterfaces()
                   .SingleInstance();

            // Application/Loops
            builder.RegisterType<OutputStateProcessingLoop>().As<IOutputStateProcessingLoop>().SingleInstance();
            builder.RegisterType<ScheduleDeterminationLoop>().As<IScheduleDeterminationLoop>().SingleInstance();
            builder.RegisterType<TemperatureReadingLoop>().As<ITemperatureReadingLoop>().SingleInstance();
            builder.RegisterType<DigitalInputReadingLoop>().As<IDigitalInputReadingLoop>().SingleInstance();

            // Application/Loops/Processing
            builder.RegisterType<HysteresisProcessor>().As<IHysteresisProcessor>().SingleInstance();
            builder.RegisterType<OutputsWriter>().As<IOutputsWriter>().SingleInstance();
            builder.RegisterType<PowerSupplySignalProcessor>().As<IPowerSupplySignalProcessor>().SingleInstance();
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
