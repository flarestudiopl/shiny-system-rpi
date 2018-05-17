using System;

namespace Domain.StorageDatabase
{
    public class User
    {
        public int UserId { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string LastSeenIpAddress { get; set; }

        public long? LastLogonDateTime { get; set; }

        public DateTime? LastLogon => DomainModelHelpers.ParseDateTime(LastLogonDateTime);

        public int IsActiveBool { get; set; }

        public bool IsActive => DomainModelHelpers.ParseBool(IsActiveBool);

        public int IsBrowseableBool { get; set; }

        public bool IsBrowseable => DomainModelHelpers.ParseBool(IsBrowseableBool);

        public int CreatedBy { get; set; }

        public long CreatedDateTime { get; set; }

        public DateTime Created => DomainModelHelpers.ParseDateTime(CreatedDateTime);

        public int? DisabledBy { get; set; }

        public long? DisabledDateTime { get; set; }

        public DateTime? Disabled => DomainModelHelpers.ParseDateTime(DisabledDateTime);
    }
}
