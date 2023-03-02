namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

public record SoftswissGamesApiUser(
    string Id,
    string Nickname,
    string Firstname = "",
    string Lastname = "",
    string City = "Moscow",
    string Country = "UA",
    string DateOfBirth = "1980-12-26",
    string Gender = "m",
    string RegisteredAt = "2018-10-11");