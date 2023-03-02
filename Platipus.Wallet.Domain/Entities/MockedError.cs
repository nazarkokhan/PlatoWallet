namespace Platipus.Wallet.Domain.Entities;

using System.Net;
using Abstract.Generic;
using Enums;

public class MockedError : Entity<int>
{
    public MockedErrorMethod Method { get; set; }

    public string Body { get; set; } = null!;

    public HttpStatusCode HttpStatusCode { get; set; }

    public string ContentType { get; set; } = null!;

    public int Count { get; set; }

    public int ExecutionOrder { get; set; }

    public TimeSpan? Timeout { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}