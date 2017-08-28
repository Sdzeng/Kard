using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:role
        public class KroleEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public KroleEntity()
        {
        
        }
        
    
        public int Id{get; set;}  
          public string Description{get; set;}  
          public bool IsStatic{get; set;}  
     
           
    }
    
}