namespace PlatipusWallet.Api.Application.Services;

public interface IGamesApiService
{
}

public class GamesApiService : IGamesApiService
{
    public void GetGamesRequest()
    {
    }
}

public class AuthInterceptor : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        
        
        return await base.SendAsync(request, cancellationToken);
    }
}