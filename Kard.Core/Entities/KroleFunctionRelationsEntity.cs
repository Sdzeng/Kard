using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:role_function_relations
        public class KroleFunctionRelationsEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public KroleFunctionRelationsEntity()
        {
        
        }
        
    
        public int KroleId{get; set;}  
          public int FunctionId{get; set;}  
           
    }
    
}