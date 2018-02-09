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



        public string Content { get; set; }

        public string SimpleContent
        {
            get
            {
              var tt=  _regex.Replace(Content,"");
                return tt;
            }
        }

        public string Location { get; set; }
        public int RepostNum { get; set; }
        public int CommentNum { get; set; }
        public int LikeNum { get; set; }

        public long? ParentEssayId { get; set; }

        public KuserEntity Kuser;

        public List<MediaEntity> MediaList;

        public List<TagEntity> TagList;

    }

}