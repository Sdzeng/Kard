using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:user_follower
        public class KuserFollowerEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public KuserFollowerEntity()
        {
        
        }
        
    
        public int KuserId{get; set;}  
          public int FollowerId{get; set;}  
          public string FollowType{get; set;}  
           
    }
    
}