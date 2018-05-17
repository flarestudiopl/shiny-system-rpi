using System;

namespace Domain.StorageDatabase
{
    public class User
    {
        public int UserId { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string LastSeenIpAddress { get; set; }

        public int? LastLogonDateTime { get; set; }

        public DateTime? LastLogon => DomainModelHelpers.ParseDateTime(LastLogonDateTime);

        public int IsActiveBool { get; set; }

        public bool IsActive => DomainModelHelpers.ParseBool(IsActiveBool);

        public int IsBrowseableBool { get; set; }

        public bool IsBrowseable => DomainModelHelpers.ParseBool(IsBrowseableBool);
    }
}
