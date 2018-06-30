namespace Commons.Localization
{
    public interface IValidationMessage
    {
        string EndTimeShouldBeGreaterBeginTime { get; }
        string MissingDaysOfWeek { get; }
        string UnknownZoneId { get; }
        string SetPointIsForTemperatureControlledZoneOnly { get; }
        string ScheduleItemOverlaps { get; }
    }

    public class ValidationMessagePl : IValidationMessage
    {
        public string EndTimeShouldBeGreaterBeginTime => "Czas zakończenia powinien być większy od czasu rozpoczęcia.";
        public string MissingDaysOfWeek => "Proszę wybrać co najmniej jeden dzień tygodnia.";
        public string UnknownZoneId => "Nieznana strefa o identyfikatorze '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Próg temperatury jest dozwolony tylko w strefach z czujnikiem temperatury.";
        public string ScheduleItemOverlaps => "Wybrane parametry nachodzą na istniejący już wpis w kalendarzu.";
    }

    public class ValidationMessageEn : IValidationMessage
    {
        public string EndTimeShouldBeGreaterBeginTime => "End time should be greater than start time";
        public string MissingDaysOfWeek => "Please specify at least one day of week to add new schedule item.";
        public string UnknownZoneId => "Unknown zone with id '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Set point should be set for temperature controlled zone only.";
        public string ScheduleItemOverlaps => "Given schedule parameters overlaps existing item.";
    }
}
