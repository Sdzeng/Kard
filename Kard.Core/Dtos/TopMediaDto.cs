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

        public int ShareNum { get; set; }

        public int LikeNum { get; set; }

        public int BrowseNum { get; set; }

        public int CommentNum { get; set; }

        public string AvatarUrl { get; set; }

        public string Location { get; set; }

        public string Title { get; set; }

        public string CreatorUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public string CreatorNickName { get; set; }

        public int MediaCount { get; set; }

        public string CdnPath { get; set; }

        public string MediaType { get; set; }

        public string MediaExtension { get; set; }







        public List<TagEntity> TagList;

    }
}
