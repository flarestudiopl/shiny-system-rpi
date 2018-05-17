using System;

namespace Domain.StorageDatabase
{
    internal static class DomainModelHelpers
    {
        public static bool ParseBool(int boolFieldValue)
        {
            return boolFieldValue == 1;
        }

        public static DateTime ParseDateTime(long dateTimeFieldValue)
        {
            return new DateTime(dateTimeFieldValue);
        }

        public static DateTime? ParseDateTime(long? dateTimeFieldValue)
        {
            return dateTimeFieldValue.HasValue ? ParseDateTime(dateTimeFieldValue.Value) : (DateTime?)null;
        }
    }
}
