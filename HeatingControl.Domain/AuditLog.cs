using System;
using System.Collections.Generic;

namespace Domain
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string TableName { get; set; }
        public DateTime EventTs { get; set; }
        public IDictionary<string, object> KeyValues { get; set; } = new Dictionary<string, object>();
        public IDictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
        public IDictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();
    }
}
