using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class EssayDto
    {
 

        public long Id { get; set; }

        public string Title { get; set; }

        public string CoverMediaType { get; set; }

        public string CoverPath { get; set; }

        public string CoverExtension { get; set; }

        public string Content { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public bool IsOriginal { get; set; }
        public decimal Score { get; set; }

        public long ScoreHeadCount { get; set; }

        public int ShareNum { get; set; }
        public int BrowseNum { get; set; }
        public int CommentNum { get; set; }
        public int LikeNum { get; set; }


        public string KuserNickName;

        public string KuserIntroduction;

        public string KuserAvatarUrl;

        public bool IsLike;

        public IList<TagEntity> TagList;

    }
}
