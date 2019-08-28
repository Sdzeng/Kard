using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    [Serializable]
    public class WeChatUserDto
    {
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }

        public string City { get; set; }

        public string Language { get; set; }

        public int Gender { get; set; }

    }
}
