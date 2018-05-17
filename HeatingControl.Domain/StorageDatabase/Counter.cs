using System;

namespace Domain.StorageDatabase
{
    public class Counter
    {
        public int CounterId { get; set; }

        public int HeaterId { get; set; }

        public int CountedSeconds { get; set; }

        public long StartDateTime { get; set; }

        public DateTime Start => DomainModelHelpers.ParseDateTime(StartDateTime);

        public long? ResetDateTime { get; set; }

        public DateTime? Reset => DomainModelHelpers.ParseDateTime(ResetDateTime);

        public int? ResettedBy { get; set; }
    }
}
