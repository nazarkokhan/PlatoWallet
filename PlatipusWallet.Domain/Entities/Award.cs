namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Award : Entity<string>
{
    public DateTime ValidUntil { get; set; }

    // public string? GameId { get; set; }

    public AwardRound? AwardRound { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}