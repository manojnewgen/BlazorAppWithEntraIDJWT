using BlazorAppWithEntraIDJWT.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorAppWithEntraIDJWT.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(HttpClient httpClient,
                                    ILocalStorageService localStorageService,
                                    AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
        /// <summary>
        ///Call the API to login
        ///Save the token in local storage
        ///Notify the AuthenticationStateProvider
        ///Return the authenticated user
        ///set httpsClient headers
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AuthenticatedUser> Login(AuthenticationUser user)
        {

            // Call the API to login
            // Save the token in local storage
            // Notify the AuthenticationStateProvider
            // Return the authenticated user
            //set httpsClient headers

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string,string>("username", user.Email),
                new KeyValuePair<string,string>("password", user.Password)
            });

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/Auth/authenticate-custom");
            requestMessage.Content = data;
            var response = await _httpClient.SendAsync(requestMessage);
            var authContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            var authUser = JsonSerializer.Deserialize<AuthenticatedUser>(authContent,
                                              new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            await _localStorageService.SetItemAsync("authToken", authUser.Access_Token);
            ((AuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(authUser.Access_Token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authUser.Access_Token);

            return authUser;

        }
        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}

  
