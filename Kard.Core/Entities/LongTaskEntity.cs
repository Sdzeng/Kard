using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    public class LongTaskEntity : DeletionAuditedEntity, IDeletionAuditedEntity
    {
        public long Id { get; set; }

       public bool IsLongTerm { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TaskDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Week { get; set; }
        public string Content { get; set; }
        public bool IsRemind { get; set; }

    }


}
