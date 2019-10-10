using System;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public int UserId { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string LastSeenIpAddress { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsBrowseable { get; set; }
        
        public int? CreatedByUserId { get; set; }

        public User CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? DisabledByUserId { get; set; }

        public User DisabledBy { get; set; }

        public DateTime? DisabledDate { get; set; }

        public string QuickLoginPinHash { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
