using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public interface IDeletionAuditedEntity
    {
        long? DeleterUserId { get; set; }
         DateTime? DeletionTime { get; set; }
         bool IsDeleted { get; set; }
    }
}
