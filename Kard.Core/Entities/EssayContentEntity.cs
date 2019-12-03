using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kard.Core.Entities
{
    //[Table("employee")]
    public class EssayContentEntity
    {
       

        public long EssayId { get; set; }

        public string Content { get; set; }
    }
}
