namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Request : Entity<string>
{
    public Request(string id)
    {
        Id = id;
    }

    public Request()
        : this(Guid.NewGuid().ToString())
    {
    }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}