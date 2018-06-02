using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:tag
        public class TagEntity : CreationAuditedEntity, ICreationAuditedEntity
    {
    
        public TagEntity()
        {
        
        }
        
    
        public int Id{get; set;}

        public long EssayId { get; set; }

        public string TagName{get; set;}  
           
    }
    
}