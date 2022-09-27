namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;
using Enums;

public class Transaction : Entity<string>
{
    public Transaction()
    {
        CreatedDate = DateTime.UtcNow;
    }
    
    public decimal Amount { get; set; }

    public TransactionType TransactionType { get; set; }

    public DateTime CreatedDate { get; set; }
    
    public string RoundId { get; set; } = null!;
    public Round Round { get; set; } = null!;
}