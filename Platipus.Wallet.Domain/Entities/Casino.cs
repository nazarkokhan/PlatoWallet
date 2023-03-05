namespace Platipus.Wallet.Domain.Entities;

using System.ComponentModel;
using System.Text.Json.Serialization;
using Abstract.Generic;
using Enums;

public class Casino : Entity<string>
{
    public Casino(
        string id,
        CasinoProvider provider,
        string signatureKey,
        string gameEnvironmentId = GameEnvironment.Default)
    {
        Id = id;
        Provider = provider;
        SignatureKey = signatureKey;
        GameEnvironmentId = gameEnvironmentId;
    }

    public CasinoProvider Provider { get; set; }

    public string SignatureKey { get; set; }

    public int InternalId { get; set; }

    public string GameEnvironmentId { get; set; }

    public GameEnvironment GameEnvironment { get; set; } = null!;

    public SpecificParams Params { get; set; } = new();

    public List<User> Users { get; set; } = new();

    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();

    public List<CasinoGames> CasinoGames { get; set; } = new();

    public record SpecificParams(
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string OpenboxVendorUid = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string ReevoCallerId = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string ReevoCallerPassword = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string Hub88PrivateWalletSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string Hub88PublicGameServiceSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string Hub88PrivateGameServiceSecuritySign = null!);
}