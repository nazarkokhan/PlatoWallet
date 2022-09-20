namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class Round : Entity
{
    public List<Transaction> Transactions { get; set; } = new();
}