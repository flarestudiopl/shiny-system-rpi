namespace Domain.BuildingModel
{
    public class DigitalInput
    {
        public int DigitalInputId { get; set; }

        public string ProtocolName { get; set; }

        public int DeviceId { get; set; }

        public string InputName { get; set; }

        public DigitalInputFunction Function { get; set; }

        public bool Inverted { get; set; }
    }

    public enum DigitalInputFunction
    {
        BatteryMode,
        BeginShutdown
    }
}
