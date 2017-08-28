using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    public class TestEntity : LastModificationAuditedEntity, ILastModificationAuditedEntity
    {
        public int Id { get; set; }
        public DateTime ShowDate { get; set; }
        public long MediaId { get; set; }
    }
}
