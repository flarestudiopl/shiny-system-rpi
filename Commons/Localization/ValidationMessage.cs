namespace Commons.Localization
{
    public interface IValidationMessage
    {
        string EndTimeShouldBeGreaterThanBeginTime { get; }
        string MissingDaysOfWeek { get; }
        string UnknownZoneId { get; }
        string SetPointIsForTemperatureControlledZoneOnly { get; }
        string ScheduleItemOverlaps { get; }
        string UnknownScheduleItemId { get; }
    }

    public class ValidationMessagePl : IValidationMessage
    {
        public string EndTimeShouldBeGreaterThanBeginTime => "Czas zakończenia powinien być większy od czasu rozpoczęcia.";
        public string MissingDaysOfWeek => "Proszę wybrać co najmniej jeden dzień tygodnia.";
        public string UnknownZoneId => "Nieznana strefa o identyfikatorze '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Próg temperatury jest dozwolony tylko w strefach z czujnikiem temperatury.";
        public string ScheduleItemOverlaps => "Wybrane parametry nachodzą na istniejący już wpis w kalendarzu.";
        public string UnknownScheduleItemId => "Nieznana wpis kalendarza o identyfikatorze '{0}'.";
    }

    public class ValidationMessageEn : IValidationMessage
    {
        public string EndTimeShouldBeGreaterThanBeginTime => "End time should be greater than start time";
        public string MissingDaysOfWeek => "Please specify at least one day of week to add new schedule item.";
        public string UnknownZoneId => "Unknown zone with id '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Set point should be set for temperature controlled zone only.";
        public string ScheduleItemOverlaps => "Given schedule parameters overlaps existing item.";
        public string UnknownScheduleItemId => "Unknown schedule item with id '{0}'.";
    }
}
