using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class UserNewsDto
    {
        public string NewsType { get; set; }
        public long Id { get; set; }

  
        public DateTime CreationTime { get; set; }

        public string EssayTitle { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
