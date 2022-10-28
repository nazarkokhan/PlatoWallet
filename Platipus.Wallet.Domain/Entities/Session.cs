namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class Session : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }
}