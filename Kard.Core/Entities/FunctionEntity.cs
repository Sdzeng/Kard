using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:function
        public class FunctionEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public FunctionEntity()
        {
        
        }
        
    
        public int Id{get; set;}  
          public string Description{get; set;}  
          public int ModuleId{get; set;}  
          public bool IsEnable{get; set;}  
          public int Sort{get; set;}  
           
    }
    
}