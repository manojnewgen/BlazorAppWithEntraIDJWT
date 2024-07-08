namespace BlazorAppWithEntraIDJWT.Login
{
    public class HybridAuthState
    {
        public string? CustomJwt { get; set; }
        public string? IdentityProviderToken { get; set; }
        public string? Username { get; set; }
        public string AccessToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
        public AuthenticationMethod AuthMethod { get; set; }

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }

    public enum AuthenticationMethod
    {
        Custom,
        IdentityProvider
    }
}

