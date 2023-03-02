namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Request : Entity<string>
{
    public Request(string? id = null)
    {
        if (id is not null)
            Id = id;
    }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}