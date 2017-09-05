using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    //Table:topic
    public class TopicEntity : LastModificationAuditedEntity, ILastModificationAuditedEntity
    {

        public TopicEntity()
        {

        }


        public int Id { get; set; }
        public string Name { get; set; }

        public long Hot { get; set; }

    }

}