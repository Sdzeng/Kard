using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class EssayCommentDto
    {
        public long Id { get; set; }

        public long? ParentId { get; set; }

        public long EssayId { get; set; }

        public string Content { get; set; }

        public long LikeNum { get; set; }

        public DateTime CreationTime { get; set; }

        public string KuserAvatarUrl { get; set; }
 
        public string KuserNickName { get; set; }


        public IEnumerable<EssayCommentDto> ParentCommentDtoList { get; set; }

        public string EssayTitle { get; set; }

        public string EssayPageUrl { get; set; }
    }
}
