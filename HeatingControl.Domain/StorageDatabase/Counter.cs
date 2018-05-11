using System;

namespace Domain.StorageDatabase
{
    public class Counter
    {
        public int CounterId { get; set; }

        public int HeaterId { get; set; }

        public int CountedSeconds { get; set; }

        public long StartDateTime { get; set; }

        public DateTime Start => new DateTime(StartDateTime);

        public long? ResetDateTime { get; set; }

        public DateTime? Reset => ResetDateTime.HasValue ? new DateTime(ResetDateTime.Value) : (DateTime?)null;

        public int? ResettedBy { get; set; }
    }
}
