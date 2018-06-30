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
    }
}
