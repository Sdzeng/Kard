using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class CoverDto
    {


        public int Id { get; set; }
        public DateTime ShowDate { get; set; }
        public long EssayId { get; set; }




        public string EssayCoverMediaType { get; set; }
        public string EssayCoverPath { get; set; }
        public string EssayCoverExtension { get; set; }

        public string EssayTitle { get; set; }
        public string EssayContent { get; set; }
        public string EssayLocation { get; set; }
        public DateTime EssayCreationTime { get; set; }


        public string KuserNickName;

        public string KuserIntroduction;

        public string KuserAvatarUrl;
    }
}
