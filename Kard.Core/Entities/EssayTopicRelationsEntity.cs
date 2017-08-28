using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities 
{
         //Table:essay_topic_relations
        public class EssayTopicRelationsEntity : LastModificationAuditedEntity,ILastModificationAuditedEntity
    {
    
        public EssayTopicRelationsEntity()
        {
        
        }
        
    
        public long EssayId{get; set;}  
          public int TopicId{get; set;}  
           
    }
    
}