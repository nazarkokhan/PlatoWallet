namespace Platipus.Wallet.Domain.Entities;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Abstract.Generic;
using Enums;

public class Casino : Entity<string>
{
    public Casino(
        string id,
        WalletProvider provider,
        string signatureKey)
    {
        Id = id;
        Provider = provider;
        SignatureKey = signatureKey;
        GameEnvironmentId = "test";
    }

    public WalletProvider Provider { get; set; }

    public string SignatureKey { get; set; }

    public int InternalId { get; set; }

    //TODO make migration.
    [Obsolete]
    public string GameEnvironmentId { get; set; }

    [Obsolete]
    public GameEnvironment GameEnvironment { get; set; } = null!;

    public SpecificParams Params { get; set; } = new();

    public List<User> Users { get; set; } = new();

    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();

    public List<CasinoGames> CasinoGames { get; set; } = new();

    public List<CasinoGameEnvironments> CasinoGameEnvironments { get; set; } = new();

    public record SpecificParams(

        // ReSharper disable once InconsistentNaming
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        int? ISoftBetProviderId = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? OpenboxVendorUid = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? ReevoCallerId = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? ReevoCallerPassword = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? Hub88PrivateWalletSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? Hub88PublicGameServiceSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? Hub88PrivateGameServiceSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? EmaraPlayProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? AtlasProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? UranusProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? EvenbetProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? AnakatechProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? SynotProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? SynotApiKey = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? VegangsterProvider = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? VegangsterPublicGameServerSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? VegangsterPrivateGameServerSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? VegangsterPrivateWalletSecuritySign = null!,
        [property: DefaultValue(null), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), NotNull]
        string? MicrogameProvider = null!);
}