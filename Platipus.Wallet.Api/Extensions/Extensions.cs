namespace Platipus.Wallet.Api.Extensions;

using Microsoft.Extensions.Options;

public static class Extensions
{
    public static IServiceCollection Configure<TOption>(this IServiceCollection serviceCollection, Func<TOption> option)
        where TOption : class
        => serviceCollection
            .AddSingleton<IOptions<TOption>>(new OptionsWrapper<TOption>(option()));
}