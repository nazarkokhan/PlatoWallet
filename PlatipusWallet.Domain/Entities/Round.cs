namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class Round : Entity
{
    public string TransactionId { get; set; } = null!;
}