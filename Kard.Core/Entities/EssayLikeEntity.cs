using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
    public class EssayLikeEntity : CreationAuditedEntity
    {
        public long Id { get; set; }

        public long EssayId { get; set; }

    }
}
