using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:tag
        public class TagEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public TagEntity()
        {
        
        }
        
    
        public int Id{get; set;}  
          public string Name{get; set;}  
           
    }
    
}