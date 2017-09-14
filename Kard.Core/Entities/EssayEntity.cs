using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    //Table:essay
    public class EssayEntity : DeletionAuditedEntity, IDeletionAuditedEntity
    {

        public EssayEntity()
        {

        }


        public long Id { get; set; }

        public string Title { get; set; }
    
        public string Content { get; set; }
        public string Location { get; set; }
        public int RepostNum { get; set; }
        public int CommentNum { get; set; }
        public int GoodNum { get; set; }
 
        public long? ParentEssayId { get; set; }


    }

}