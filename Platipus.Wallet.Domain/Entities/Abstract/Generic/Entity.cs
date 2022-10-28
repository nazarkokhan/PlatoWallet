namespace Platipus.Wallet.Domain.Entities.Abstract.Generic;

public abstract class Entity<T>
{
    public T Id { get; set; } = default!;
}