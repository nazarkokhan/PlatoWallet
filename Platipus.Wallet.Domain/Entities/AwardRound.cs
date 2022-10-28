namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class AwardRound : Entity
{
    public string AwardId { get; set; } = null!;
    public Award Award { get; set; } = null!;

    public string RoundId { get; set; } = null!;
    public Round Round { get; set; } = null!;
}