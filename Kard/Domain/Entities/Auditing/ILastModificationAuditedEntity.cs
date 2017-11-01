using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public interface ILastModificationAuditedEntity
    {
        long? LastModifierUserId { get; set; }
        DateTime? LastModificationTime { get; set; }

    }
}
