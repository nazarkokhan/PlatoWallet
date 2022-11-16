namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Request : Entity<string>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

public class Round : Entity<string>
{
    public bool Finished { get; set; }

    public AwardRound? AwardRound { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = new();
}