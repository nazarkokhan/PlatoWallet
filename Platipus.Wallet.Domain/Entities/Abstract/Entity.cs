namespace Platipus.Wallet.Domain.Entities.Abstract;

using Generic;

public abstract class Entity : Entity<Guid>
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}