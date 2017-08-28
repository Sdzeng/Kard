using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities
{
    public interface IKeyEntity<TPrimaryKey> : IEntity
    {
        TPrimaryKey Id { get; set; }
    }
}
