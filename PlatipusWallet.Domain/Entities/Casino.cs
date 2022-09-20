namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Casino : Entity<string>
{
    public string SignatureKey { get; set; } = null!;
}