using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:module
        public class ModuleEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public ModuleEntity()
        {
        
        }
        
    
        public int Id{get; set;}  
          public string Description{get; set;}  
          public int Sort{get; set;}  
          public int ParentId{get; set;}  
           
    }
    
}