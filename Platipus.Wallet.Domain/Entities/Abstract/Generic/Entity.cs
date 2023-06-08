namespace Platipus.Wallet.Domain.Entities.Abstract.Generic;

public abstract class Entity<T> : AuditableEntity
{
    public T Id { get; set; } = default!;
}