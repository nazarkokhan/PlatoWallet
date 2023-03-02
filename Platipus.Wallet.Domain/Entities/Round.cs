namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Round : Entity<string>
{
    public Round(string id)
    {
        Id = id;
    }
    public string InternalId { get; set; } = null!;

    public bool Finished { get; set; }

    public AwardRound? AwardRound { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = new();

    public void Finish()
    {
        Finished = true;
    }
}