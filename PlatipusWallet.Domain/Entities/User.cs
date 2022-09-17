#pragma warning disable CS8618
namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class User : Entity
{
    public string Currency { get; set; }

    public List<Game> Games { get; set; } = new();
}