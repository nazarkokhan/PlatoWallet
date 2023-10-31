using Platipus.Wallet.Api.Application.Results.Base;
using Platipus.Wallet.Api.Application.Results.Base.WithData;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium;

public sealed record SweepiumResult : BaseResult<SweepiumErrorCode>, ISweepiumResult
{
}