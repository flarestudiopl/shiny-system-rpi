using HeatingControl.Domain;

namespace HeatingControl.Extensions
{
    public static class DomainExtensions
    {
        public static bool IsTemperatureControlled(this Zone zone) => zone.TemperatureControlledZone != null;
    }
}
