namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class CasinoCurrencies : Entity
{
    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;

    public string CurrencyId { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}