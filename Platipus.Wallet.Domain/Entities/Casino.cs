namespace Platipus.Wallet.Domain.Entities;

using System.Text.Json.Nodes;
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

    // public JsonNode Params { get; set; } = DefaultParams;
    public Dictionary<string, JsonNode> Params { get; set; } = new();

    public List<User> Users { get; set; } = new();

    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();

    public List<CasinoGames> CasinoGames { get; set; } = new();

    // private static readonly JsonNode DefaultParams = JsonNode.Parse("{}")!;
}