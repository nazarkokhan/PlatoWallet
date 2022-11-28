namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Transaction : Entity<string>
{
    public Transaction()
    {
        CreatedDate = DateTime.UtcNow;
        InternalId = Guid.NewGuid().ToString();
    }

    public decimal Amount { get; set; }

    public string InternalId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string RoundId { get; set; } = null!;
    public Round Round { get; set; } = null!;
}