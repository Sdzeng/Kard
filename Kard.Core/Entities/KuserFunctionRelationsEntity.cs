using Kard.Domain.Entities.Auditing;
using System;

namespace Kard.Core.Entities
{
    //Table:user_function_relations
    public class KuserFunctionRelationsEntity : LastModificationAuditedEntity, ILastModificationAuditedEntity
    {

        public KuserFunctionRelationsEntity()
        {

        }


        public int KuserId { get; set; }
        public int FunctionId { get; set; }
        public bool Effect { get; set; }

    }

}