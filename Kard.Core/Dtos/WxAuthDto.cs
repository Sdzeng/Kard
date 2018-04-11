using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class WxAuthDto
    {
        public string openid { get; set; }

        public string unionid { get; set; }

        public string session_key { get; set; }

        public int? errcode { get; set; }

        public string errmsg { get; set; }
    }
}
