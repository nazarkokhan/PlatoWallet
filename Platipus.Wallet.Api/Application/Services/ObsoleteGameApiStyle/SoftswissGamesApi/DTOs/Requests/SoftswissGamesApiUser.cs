namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

using System.ComponentModel;

public record SoftswissGamesApiUser(
    [property: DefaultValue("username")] string Id,
    [property: DefaultValue("username")] string Nickname,
    [property: DefaultValue("email")] string Email = "email@dodik.com",
    [property: DefaultValue("dodik")] string Firstname = "dodik",
    [property: DefaultValue("bobik")] string Lastname = "bobik",
    [property: DefaultValue("Moscow")] string City = "Moscow",
    [property: DefaultValue("UA")] string Country = "UA",
    [property: DefaultValue("1980-12-26")] string DateOfBirth = "1980-12-26",
    // [property: DefaultValue("m")] string Gender = "m",
    [property: DefaultValue("2018-10-11")] string RegisteredAt = "2018-10-11");