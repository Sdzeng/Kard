using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public interface ICreationAuditedEntity : IEntity
    {
        long? Creator { get; set; }
        DateTime CreationTime { get; set; }

    }
}
