namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;

using Domain.Entities.Enums;

public record struct MockedErrorIdentifiers(
    MockedErrorMethod WalletMethod,
    string UsernameOrSession,
    bool CallerIdentifiedBySession);