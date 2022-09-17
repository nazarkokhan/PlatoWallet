namespace PlatipusWallet.Domain.Entities;

using Abstract;

#pragma warning disable CS8618
public class Game : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public List<Round> Rounds { get; set; } = new();
}