namespace PlatipusWallet.Domain.Entities;

using System.Net;
using Abstract;

public class MockedError : Entity
{
    public ErrorMockMethod Method { get; set; }

    public string Body { get; set; } = null!;

    public HttpStatusCode HttpStatusCode { get; set; }

    public string ContentType { get; set; } = null!;

    public int Count { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}