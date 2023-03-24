namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Award : Entity<string>
{
    public Award(string id, DateTime validUntil)
    {
        Id = id;
        ValidUntil = validUntil;
    }

    public DateTime ValidUntil { get; private set; }

    public string? Currency { get; set; }

    public AwardRound? AwardRound { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}