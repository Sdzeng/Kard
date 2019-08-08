using Kard.Core.Entities;
using Kard.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kard.Core.Dtos
{
    public class TopMediaDto
    {
        public int Id { get; set; }

        public string Category { get; set; }


        public decimal Score { get; set; }

        public long ScoreHeadCount { get; set; }

        public int ShareNum { get; set; }

        public int LikeNum { get; set; }

        public int BrowseNum { get; set; }

        public int CommentNum { get; set; }

        public string AvatarUrl { get; set; }

        public string Location { get; set; }

        public string Title { get; set; }

        public string SubContent { get; set; }

        public string PageUrl { get; set; }

        public string CreatorUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public string CreatorNickName { get; set; }

       

        public string CoverPath { get; set; }

        public string CoverMediaType { get; set; }

        public string CoverExtension { get; set; }







        public List<TagEntity> TagList;

    }
}
