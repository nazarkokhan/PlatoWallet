namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class User : Entity<int>
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsDisabled { get; set; }

    public string CurrencyId { get; set; } = null!;
    public Currency Currency { get; set; } = null!;

    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;

    public List<Round> Rounds { get; set; } = new();

    public List<Session> Sessions { get; set; } = new();

    public List<Award> Awards { get; set; } = new();

    public List<MockedError> MockedErrors { get; set; } = new();

    public List<Request> Requests { get; set; } = new();
}