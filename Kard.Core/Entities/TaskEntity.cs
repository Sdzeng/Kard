using DapperExtensionsCore.Mapper;
using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
   public class TaskEntity : DeletionAuditedEntity, IDeletionAuditedEntity
    {
        public long Id { get; set; }

        public long? LongTaskId { get; set; }
     
        public DateTime TaskDate { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
     
        public string Content { get; set; }
        public bool IsRemind { get; set; }

        public bool IsDone { get; set; }
    }

 
}
