namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class User : Entity
{
    public string UserName { get; set; } = null!;
    
    public string Password { get; set; } = null!;

    public decimal Balance { get; set; }
    
    public string Currency { get; set; } = null!;

    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;

    public List<Round> Rounds { get; set; } = new();
    
    public List<Session> Sessions { get; set; } = new();
}