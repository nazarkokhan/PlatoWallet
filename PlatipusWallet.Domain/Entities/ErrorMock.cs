namespace PlatipusWallet.Domain.Entities;

using System.Net;
using Abstract;

public class ErrorMock : Entity
{
    public string MethodPath { get; set; }

    public string Body { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }

    public Guid SessionId { get; set; }
    public Session Session { get; set; } = null!;
}