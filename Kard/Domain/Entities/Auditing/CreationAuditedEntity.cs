using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class CreationAuditedEntity: ICreationAuditedEntity
    {

     

        public virtual long? CreatorUserId { get; set; }
        public virtual DateTime CreationTime { get; set; }

        public virtual string CreatorUserName { get; set; }

    }
}
