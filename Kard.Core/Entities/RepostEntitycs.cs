using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
    public class RepostEntity: DeletionAuditedEntity
    {

        public long Id { get; set; }

        public long EssayId { get; set; }

    }
}
