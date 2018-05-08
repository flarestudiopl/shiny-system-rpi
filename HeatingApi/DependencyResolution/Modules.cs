﻿using Autofac;
using HardwareAccess.Buses;
using HardwareAccess.Buses.PlatformIntegration;
using HardwareAccess.Devices;
using HeatingControl.Application;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Loops;
using HeatingControl.Application.Loops.Processing;
using HeatingControl.Application.Queries;
using Microsoft.Extensions.Hosting;
using Storage.BuildingModel;

namespace HeatingApi.DependencyResolution
{
    public class Modules
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterPersistence(builder);
            RegisterHardwareAccess(builder);
            RegisterControl(builder);
        }

        private static void RegisterPersistence(ContainerBuilder builder)
        {
            // BuildingModel
            builder.RegisterType<BuildingModelProvider>().As<IBuildingModelProvider>().SingleInstance();
            builder.RegisterType<BuildingModelSaver>().As<IBuildingModelSaver>().SingleInstance();
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
#if DEBUG
            builder.RegisterType<HardwareAccess.DummyDevices.PowerOutput>().As<IPowerOutput>().SingleInstance();
            builder.RegisterType<HardwareAccess.DummyDevices.TemperatureSensor>().As<ITemperatureSensor>().SingleInstance();
#else
            builder.RegisterType<PowerOutput>().As<IPowerOutput>().SingleInstance();
            builder.RegisterType<TemperatureSensor>().As<ITemperatureSensor>().SingleInstance();
#endif
        }

        private static void RegisterControl(ContainerBuilder builder)
        {
            builder.RegisterType<HeatingControl>()
                   .As<IHeatingControl>()
                   .As<IHostedService>()
                   .SingleInstance();

            // Application
            builder.RegisterType<ControllerStateBuilder>().As<IControllerStateBuilder>().SingleInstance();

            // Application/Commands
            builder.RegisterType<TemperatureSetPointExecutor>().As<ITemperatureSetPointExecutor>().SingleInstance();

            // Application/Loops
            builder.RegisterType<OutputStateProcessingLoop>().As<IOutputStateProcessingLoop>().SingleInstance();
            builder.RegisterType<ScheduleDeterminationLoop>().As<IScheduleDeterminationLoop>().SingleInstance();
            builder.RegisterType<TemperatureReadingLoop>().As<ITemperatureReadingLoop>().SingleInstance();

            // Application/Loops/Processing
            builder.RegisterType<HysteresisProcessor>().As<IHysteresisProcessor>().SingleInstance();
            builder.RegisterType<UsageCollector>().As<IUsageCollector>().SingleInstance();
            builder.RegisterType<ZoneTemperatureProvider>().As<IZoneTemperatureProvider>().SingleInstance();

            // Application/Queries
            builder.RegisterType<AvailableHeatersProvider>().As<IAvailableHeatersProvider>().SingleInstance();
            builder.RegisterType<AvailableTemperatureSensorsProvider>().As<IAvailableTemperatureSensorsProvider>().SingleInstance();
            builder.RegisterType<DashboardSnapshotProvider>().As<IDashboardSnapshotProvider>().SingleInstance();
            builder.RegisterType<ZoneDetailsProvider>().As<IZoneDetailsProvider>().SingleInstance();
            builder.RegisterType<ZoneSettingsProvider>().As<IZoneSettingsProvider>().SingleInstance();
        }
    }
}
