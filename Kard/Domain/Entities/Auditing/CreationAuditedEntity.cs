using System;

namespace Kard.Domain.Entities.Auditing
{
    public class CreationAuditedEntity: ICreationAuditedEntity
    {

     

        public virtual long? CreatorUserId { get; set; }
        public virtual DateTime CreationTime { get; set; }

        //public virtual string CreatorUserName { get; set; }


        public void AuditCreation(long userId) {
            this.CreatorUserId = userId;
            this.CreationTime = DateTime.Now;
       
        }
    }

 
}
