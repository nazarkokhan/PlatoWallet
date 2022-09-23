namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class CasinoCurrencies : Entity
{
    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;
    
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
}