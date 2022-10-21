namespace PlatipusWallet.Domain.Entities;

using System.Net;
using Abstract;

public class ErrorMock : Entity
{
    public string MethodPath { get; set; } = null!;

    public string Body { get; set; } = null!;

    public HttpStatusCode HttpStatusCode { get; set; }

    public Guid SessionId { get; set; }
    public Session Session { get; set; } = null!;
}

// public class MockedError : Entity
// {
//     public ErrorMockMethod Method { get; set; }
//
//     public string Body { get; set; } = null!;
//
//     public HttpStatusCode HttpStatusCode { get; set; }
//     
//     public string ContentType { get; set; } = null!;
//
//     public int Count { get; set; }
//
//     public Guid UserId { get; set; }
//     public User User { get; set; } = null!;
// }
//
// public enum ErrorMockMethod
// {
//     Balance,
//     Bet,
//     Win,
//     Award,
//     Rollback
// }