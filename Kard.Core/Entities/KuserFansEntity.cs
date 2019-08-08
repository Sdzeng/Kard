using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:user_follower
        public class KuserFansEntity : CreationAuditedEntity, ICreationAuditedEntity
    {
    
        public KuserFansEntity()
        {
        
        }

        public long Id { get; set; }
        public int BeConcernedUserId { get; set;}  
     
           
    }
    
}