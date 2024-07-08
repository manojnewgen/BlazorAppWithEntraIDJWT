using Blazored.LocalStorage;
using System.Net.Http.Headers;

public class AuthorizationMessageHandler : DelegatingHandler
{
    /// <summary>
    /// AuthorizationMessageHandler: Ensures that outgoing API requests include the necessary authorization token (Bearer token) for authenticated users.
    /// </summary>
    private readonly ILocalStorageService _localStorage;

    public AuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
