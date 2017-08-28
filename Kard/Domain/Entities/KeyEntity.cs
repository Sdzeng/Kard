using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities
{
    public class KeyEntity<TPrimaryKey>: IKeyEntity<TPrimaryKey>
    {
       public virtual TPrimaryKey Id { get; set; }
    }
}
