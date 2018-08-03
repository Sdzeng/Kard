using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kard.Core.Entities
{
    //Table:essay
    public class EssayEntity : DeletionAuditedEntity, IDeletionAuditedEntity
    {
        private static readonly Regex _regex = new Regex(@"(?'group1'#)([^#]+?)(?'-group1'#)");

        public EssayEntity()
        {

        }


        public long Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public bool IsOriginal { get; set; }
        public decimal Score { get; set; }
        public int ShareNum { get; set; }
        public int BrowseNum { get; set; }
        public int CommentNum { get; set; }
        public int LikeNum { get; set; }

        public long? ParentEssayId { get; set; }

        public KuserEntity Kuser;

        public EssayLikeEntity EssayLike;

        public List<MediaEntity> MediaList;

        public List<TagEntity> TagList;

    }

}