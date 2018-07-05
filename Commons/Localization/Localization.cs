using System.Globalization;

namespace Commons.Localization
{
    public static class Localization
    {
        private static readonly IValidationMessage ValidationMessagePl = new ValidationMessagePl();
        private static readonly IValidationMessage ValidationMessageEn = new ValidationMessageEn();

        public static IValidationMessage ValidationMessage
        {
            get
            {
                switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                {
                    case "pl":
                        return ValidationMessagePl;
                    default:
                        return ValidationMessageEn;
                }
            }
        }

        private static readonly INotificationMessage NotificationMessagePl = new NotificationMessagePl();
        private static readonly INotificationMessage NotificationMessageEn = new NotificationMessageEn();

        public static INotificationMessage NotificationMessage
        {
            get
            {
                return NotificationMessagePl;
                
                // TODO
                switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                {
                    case "pl":
                        return NotificationMessagePl;
                    default:
                        return NotificationMessageEn;
                }
            }
        }
    }
}
