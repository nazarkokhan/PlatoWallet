namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class Transaction : Entity
{
    public Guid RoundId { get; set; }
    public Round Round { get; set; } = null!;
}