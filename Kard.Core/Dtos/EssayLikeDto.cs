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

        public KuserEntity Kuser;
    }
}
