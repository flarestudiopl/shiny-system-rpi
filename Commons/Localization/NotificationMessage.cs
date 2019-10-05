namespace Commons.Localization
{
    public interface INotificationMessage
    {
        string ScheduledRemovedDueToControlTypeChange { get; }
        string DisablingAllOutputs { get; }
        string StartingControlLoops { get; }
        string SendingCancellationToControlLoops { get; }
        string SkippingHeaterWithoutName { get; }
        string SkippingSensorWithoutName { get; }
        string SkippingZoneWithoutName { get; }
        string SkippingPowerZoneWithoutName { get; }
        string TemperatureValueTooOld { get; }
        string NoTemperatureData { get; }
        string LoopFailed { get; }
        string SensorCrcError { get; }
        string PowerOffRequested { get; }
        string HeaterRemovedFromPowerZoneDueToUsageUnitChange { get; }
    }

    public class NotificationMessagePl : INotificationMessage
    {
        public string ScheduledRemovedDueToControlTypeChange => "Terminarz strefy '{0}' został usunięty z powodu zmiany sposobu sterowania.";
        public string DisablingAllOutputs => "Wyłączanie wszystkich wyjść...";
        public string StartingControlLoops => "Uruchamianie pętli sterujących...";
        public string SendingCancellationToControlLoops => "Żądanie zatrzymania pętli sterujących...";
        public string SkippingHeaterWithoutName => "Pomijam ogrzewacz bez nazwy.";
        public string SkippingSensorWithoutName => "Pomijam czujnik bez nazwy.";
        public string SkippingZoneWithoutName => "Pomijam strefę bez nazwy.";
        public string SkippingPowerZoneWithoutName => "Pomijam strefę zasilania bez nazwy.";
        public string TemperatureValueTooOld => "Odczyt temperatury strefy '{0}' jest przedawniony. Proaktywne odłączenie zasilania.";
        public string NoTemperatureData => "Brak danych z czujnika '{0}' w strefie '{1}'. Proaktywne odłączenie zasilania.";
        public string LoopFailed => "Awaria pętli '{0}'! Ponowna próba za {0} sekund.";
        public string SensorCrcError => "Błąd CRC czujnika '{0}'. Pomijam odczyt.";
        public string PowerOffRequested => "Rozpoczęto procedurę wyłączania sterownika...";
        public string HeaterRemovedFromPowerZoneDueToUsageUnitChange => "Ogrzewacz '{0}' został odłączony od strefy zasilania z powodu zmiany jednostki zużycia.";
    }

    public class NotificationMessageEn : INotificationMessage
    {
        public string ScheduledRemovedDueToControlTypeChange => "Schedule in zone '{0}' was removed due to control type change.";
        public string DisablingAllOutputs => "Disabling all outputs...";
        public string StartingControlLoops => "Starting control loops...";
        public string SendingCancellationToControlLoops => "Sending cancellation to control loops...";
        public string SkippingHeaterWithoutName => "Skipping heater without name.";
        public string SkippingSensorWithoutName => "Skipping sensor without name.";
        public string SkippingZoneWithoutName => "Skipping zone without name.";
        public string SkippingPowerZoneWithoutName => "Skipping power zone without name.";
        public string TemperatureValueTooOld => "Temperature value for zone '{0}' is too old. Proactive power cutoff.";
        public string NoTemperatureData => "No temperature data for sensor '{0}' in zone '{1}'. Proactive power cutoff.";
        public string LoopFailed => "{0} loop failed! Retrying in {0} seconds.";
        public string SensorCrcError => "Sensor '{0}' CRC error. Skipping readout.";
        public string PowerOffRequested => "Shutdown sequence started...";
        public string HeaterRemovedFromPowerZoneDueToUsageUnitChange => "Heater '{0}' was removed from power zone due to usage unit change.";
    }
}
