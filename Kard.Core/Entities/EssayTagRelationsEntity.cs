using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:essay_tag_relations
        public class EssayTagRelationsEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public EssayTagRelationsEntity()
        {
        
        }
        
    
        public long EssayId{get; set;}  
          public int TagId{get; set;}  
           
    }
    
}