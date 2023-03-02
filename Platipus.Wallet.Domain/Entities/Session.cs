namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Session : Entity<string>
{
    public Session()
        : this(TimeSpan.FromDays(10))
    {
    }

    public Session(TimeSpan duration)
    {
        ExpirationDate = DateTime.UtcNow.Add(duration);
    }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public bool IsTemporaryToken { get; set; }
}