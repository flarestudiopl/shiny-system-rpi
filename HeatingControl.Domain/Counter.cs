using System;

namespace Domain
{
    public class Counter
    {
        public int CounterId { get; set; }

        public int HeaterId { get; set; }

        public int CountedSeconds { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ResetDate { get; set; }

        public int? ResettedByUserId { get; set; }

        public User ResettedBy { get; set; }
    }
}
