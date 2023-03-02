namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class GameEnvironment : Entity<string>
{
    public const string Default = "test";
    public const string Local = "local";

    public GameEnvironment(string id, Uri baseUrl, Uri uisBaseUrl)
    {
        Id = id;
        BaseUrl = baseUrl;
        UisBaseUrl = uisBaseUrl;
    }

    public Uri BaseUrl { get; set; }

    public Uri UisBaseUrl { get; set; }

    public List<Casino> Casinos { get; set; } = new();
}