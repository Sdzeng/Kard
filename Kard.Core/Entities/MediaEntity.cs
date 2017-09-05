using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    //Table:media
    public class MediaEntity : CreationAuditedEntity, ICreationAuditedEntity
    {

        public MediaEntity()
        {

        }


        public long Id { get; set; }
        public long EssayId { get; set; }

        public int Sort { get; set; }

        public string MediaType { get; set; }

        public string MediaExtension { get; set; }


        public string CdnPath { get; set; }
        public long HeartNum { get; set; }


        public EssayEntity Essay;

        public KuserEntity Kuser;
    }

}