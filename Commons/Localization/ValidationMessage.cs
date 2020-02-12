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
        string UnknownUserOrWrongPassword { get; }
        string PinOrPasswordRequired { get; }
        string UserAlreadyExists { get; }
        string UserNameAndLoginShallNotBeEmpty { get; }
        string UnknownHeaterId { get; }
        string CantDeleteHeaterAssignedToZone { get; }
        string CantDeleteHeaterAssignedToPowerZone { get; }
        string UnknownPowerZoneId { get; }
        string UnknownTemperatureSensorId { get; }
        string CantDeleteSensorAssignedToZone { get; }
        string NameCantBeEmpty { get; }
        string PowerOutputParametersAlreadyAssigned { get; }
        string PowerZoneIntervalCantBeLessThan1Minute { get; }
        string PowerZoneSwitchDelayCantBeNegative { get; }
        string PowerZoneHeaterUnitMismatch { get; }
        string HeaterAlreadyInUseByAnotherPowerZone { get; }
        string PowerZoneTotalLimitLessThanTopHeaterUsage { get; }
        string DeviceIdCantBeEmpty { get; }
        string DeviceIdAlreadyInUse { get; }
        string HeaterAlreadyInUseByAnotherZone { get; }
        string UsageCantBeNegative { get; }
        string PowerLimitCantBeNegative { get; }
        string MinimumStateChangeIntervalCantBeNegative { get; }
        string IncorrectPinError { get; }
        string PasswordCannotBeEmpty { get; }
        string HysteresisTooLow { get; }
    }

    public class ValidationMessagePl : IValidationMessage
    {
        public string EndTimeShouldBeGreaterThanBeginTime => "Czas zakończenia powinien być większy od czasu rozpoczęcia.";
        public string MissingDaysOfWeek => "Proszę wybrać co najmniej jeden dzień tygodnia.";
        public string UnknownZoneId => "Nieznana strefa o identyfikatorze '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Próg temperatury jest dozwolony tylko w strefach z czujnikiem temperatury.";
        public string ScheduleItemOverlaps => "Wybrane parametry nachodzą na istniejący już wpis w kalendarzu.";
        public string UnknownScheduleItemId => "Nieznana wpis kalendarza o identyfikatorze '{0}'.";
        public string UnknownUserOrWrongPassword => "Nieznany użytkownik lub błędne hasło.";
        public string PinOrPasswordRequired => "PIN lub hasło jest wymagane.";
        public string UserAlreadyExists => "Użytkownik '{0}' już istnieje.";
        public string UserNameAndLoginShallNotBeEmpty => "Login i hasło nie mogą być puste.";
        public string UnknownHeaterId => "Nieznany ogrzewacza o identyfikatorze '{0}'.";
        public string CantDeleteHeaterAssignedToZone => "Nie można usunąć ogrzewacza przypisanego do strefy.";
        public string CantDeleteHeaterAssignedToPowerZone => "Nie można usunąć ogrzewacza przypisanego do strefy zasilania.";
        public string UnknownPowerZoneId => "Nieznana strefa zasilania o identyfikatorze '{0}'.";
        public string UnknownTemperatureSensorId => "Nieznany czujnik temperatury o identyfikatorze '{0}'.";
        public string CantDeleteSensorAssignedToZone => "Nie można usunąć czujnika temperatury przypisanego do strefy.";
        public string NameCantBeEmpty => "Nazwa nie może być pusta.";
        public string PowerOutputParametersAlreadyAssigned => "Ogrzewacz o wskazanych parametrach wyjściowych ({0}) już istnieje.";
        public string PowerZoneIntervalCantBeLessThan1Minute => "Czas przełączania strefy zasilania nie może być krótszy niż 1 minuta.";
        public string PowerZoneSwitchDelayCantBeNegative => "Opóźnienie przełącznia strefy zasilania nie może być ujemne.";
        public string PowerZoneHeaterUnitMismatch => "Niezgodna jednostka zużycia dla ogrzewacza o identyfikatorze '{0}'.";
        public string HeaterAlreadyInUseByAnotherPowerZone => "Ogrzewacz o identyfikatorze '{0}' jest już przypisany do innej strefy zasilania.";
        public string PowerZoneTotalLimitLessThanTopHeaterUsage => "Nie można dodać strefy zasilania z limitem mniejszym niż największe zużycie spośród wybranych ogrzewaczy.";
        public string DeviceIdCantBeEmpty => "Identyfikator urządzenia nie może być pusty.";
        public string DeviceIdAlreadyInUse => "Identyfikator urządzenia jest już w użyciu.";
        public string HeaterAlreadyInUseByAnotherZone => "Ogrzewacz o identyfikatorze '{0}' jest już przypisany do innej strefy.";
        public string UsageCantBeNegative => "Zużycie nie może być ujemne.";
        public string PowerLimitCantBeNegative => "Ograniczenie mocy nie może być ujemne.";
        public string MinimumStateChangeIntervalCantBeNegative => "Minimalny czas przełączania nie może być ujemny.";
        public string IncorrectPinError => "Pin powinien zawierać między 4 a 8 cyfr.";
        public string PasswordCannotBeEmpty => "Hasło nie może być puste.";
        public string HysteresisTooLow => "Za niska histereza.";
    }

    public class ValidationMessageEn : IValidationMessage
    {
        public string EndTimeShouldBeGreaterThanBeginTime => "End time should be greater than start time";
        public string MissingDaysOfWeek => "Please specify at least one day of week to add new schedule item.";
        public string UnknownZoneId => "Unknown zone with id '{0}'.";
        public string SetPointIsForTemperatureControlledZoneOnly => "Set point should be set for temperature controlled zone only.";
        public string ScheduleItemOverlaps => "Given schedule parameters overlaps existing item.";
        public string UnknownScheduleItemId => "Unknown schedule item with id '{0}'.";
        public string UnknownUserOrWrongPassword => "Unknown user or wrong password.";
        public string PinOrPasswordRequired => "Either PIN or password is required.";
        public string UserAlreadyExists => "User with login '{0}' already exists.";
        public string UserNameAndLoginShallNotBeEmpty => "User login and password shall not be empty.";
        public string UnknownHeaterId => "Unknown heater with id '{0}'.";
        public string CantDeleteHeaterAssignedToZone => "Can't delete heater assigned to zone.";
        public string CantDeleteHeaterAssignedToPowerZone => "Can't delete heater assigned to power zone.";
        public string UnknownPowerZoneId => "Unknown power zone with id '{0}'.";
        public string UnknownTemperatureSensorId => "Unknown temperature sensor with id '{0}'.";
        public string CantDeleteSensorAssignedToZone => "Can't delete temperature sensor assigned to zone.";
        public string NameCantBeEmpty => "Name can't be empty.";
        public string PowerOutputParametersAlreadyAssigned => "Heater with the same power output parameters ({0}) already exists.";
        public string PowerZoneIntervalCantBeLessThan1Minute => "Cannot set power zone interval to less than 1 minute.";
        public string PowerZoneSwitchDelayCantBeNegative => "Power zone switch delay cannot be negative.";
        public string PowerZoneHeaterUnitMismatch => "Power unit missmatch for heater of id '{0}' when creating new power zone.";
        public string HeaterAlreadyInUseByAnotherPowerZone => "Heater of id '{0}' is already in use by another power zone.";
        public string PowerZoneTotalLimitLessThanTopHeaterUsage => "Cannot add power zone with total limit less than highest usage from heaters.";
        public string DeviceIdCantBeEmpty => "Device id can't be empty.";
        public string DeviceIdAlreadyInUse => "Device id already in use.";
        public string HeaterAlreadyInUseByAnotherZone => "Heater of id '{0}' is already in use by another zone.";
        public string UsageCantBeNegative => "Usage can't be negative.";
        public string PowerLimitCantBeNegative => "Power limit can't be negative.";
        public string MinimumStateChangeIntervalCantBeNegative => "Minimum state change interval can't be negative.";
        public string IncorrectPinError => "Pin should be a number and contain between 4 and 8 digits.";
        public string PasswordCannotBeEmpty => "Password cannot be empty string.";
        public string HysteresisTooLow => "Hysteresis is too low.";
    }
}
