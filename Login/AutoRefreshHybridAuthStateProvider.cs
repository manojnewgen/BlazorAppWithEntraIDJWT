using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorAppWithEntraIDJWT.Login
{
    public class AutoRefreshHybridAuthStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public AutoRefreshHybridAuthStateProvider(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var result = await _accessTokenProvider.RequestAccessToken();

            if (result.TryGetToken(out var token))
            {
                var claims = new[] { new Claim("access_token", token.Value) };
                var identity = new ClaimsIdentity(claims, "Bearer");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }

            return new AuthenticationState(new ClaimsPrincipal());
        }

        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            return _accessTokenProvider.RequestAccessToken();
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return _accessTokenProvider.RequestAccessToken(options);
        }
    }
}