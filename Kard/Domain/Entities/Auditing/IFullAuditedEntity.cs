namespace Kard.Domain.Entities.Auditing
{
    public interface IFullAuditedEntity: IDeletionAuditedEntity, ILastModificationAuditedEntity,ICreationAuditedEntity
    {
    }
}
