namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;
using Enums;

public class Transaction : Entity<string>
{
    public Transaction(string id, decimal amount, TransactionType type)
    {
        Id = id;
        Amount = amount;
        Type = type;
    }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public bool IsCanceled { get; set; }

    public string InternalId { get; set; } = null!;

    public string RoundId { get; set; } = null!;
    public Round Round { get; set; } = null!;

    public void Cancel()
    {
        IsCanceled = true;
    }
}