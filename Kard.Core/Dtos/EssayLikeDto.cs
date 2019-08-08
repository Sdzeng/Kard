using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class EssayLikeDto
    {
        public long Id { get; set; }
        public long EssayId { get; set; }

        public DateTime CreationTime { get; set; }

        public string EssayTitle { get; set; }

        public string EssayPageUrl { get; set; }

        public string KuserId { get; set; }

        public string KuserNickName { get; set; }

        public string KuserAvatarUrl { get; set; }

        
 
    }
}
