using BlazorAppWithEntraIDJWT.Models;

namespace BlazorAppWithEntraIDJWT.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUser> Login(AuthenticationUser user);
        Task Logout();
    }
}