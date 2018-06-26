using DapperExtensionsCore.Mapper;
using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

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

    public class LongTaskMapper : ClassMapper<LongTaskEntity>
    {

        public LongTaskMapper()
        {
            Table("long_task");
            Map(e => e.IsLongTerm).Ignore();
            Map(e => e.TaskDate).Ignore();
            AutoMap();
        }
    }
}
