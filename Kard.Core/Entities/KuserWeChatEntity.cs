using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Kard.Core.Entities
{
    public class KuserWeChatEntity : CreationAuditedEntity, ICreationAuditedEntity
    {

        public int Id { get; set; }

        public long KuserId { get; set; }
        public string OpenId { get; set; }
        public string SessionKey { get; set; }

        public string UnionId { get; set; }
    }
}
