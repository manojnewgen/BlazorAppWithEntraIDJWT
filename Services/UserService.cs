using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BlazorServerApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BlazorAppWithEntraIDJWT.Login;
using System.Text.Json;
using IdentityModel.Client;
using System.Net.Http.Headers;

namespace BlazorServerApp.Services
{
    public class UserService : IUserService
    {
        public HttpClient _httpClient { get; }
        public AppSettings _appSettings { get; }

        public CustomAuthenticationStateProvider _customAuthenticationStateProvider { get; }

        public UserService(HttpClient httpClient, IOptions<AppSettings> appSettings, CustomAuthenticationStateProvider customAuthenticationStateProvider)
        {
            _appSettings = appSettings.Value;

            httpClient.BaseAddress = new Uri(_appSettings.BaseAddress);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");

            _httpClient = httpClient;
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
        }

        public async Task<User> LoginAsync(User user)
        {
            var returnedUser = new User();

            //if (!(string.IsNullOrEmpty(user.EmailAddress) && string.IsNullOrEmpty(user.Password)))
            //{
            //    user.Password = user.Password;
            //    string serializedUser = JsonConvert.SerializeObject(user);

            //    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "authenticate-custom")
            //    {
            //        Content = new StringContent(serializedUser)
            //    };
            //    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            //    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    var response = await _httpClient.SendAsync(requestMessage);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseBody = await response.Content.ReadAsStringAsync();
            //        returnedUser = JsonConvert.DeserializeObject<User>(responseBody);
            //    }
            //    else
            //    {
            //        var responseBody = await response.Content.ReadAsStringAsync();
            //        var errorMessage = $"API call failed with status code: {response.StatusCode}, response: {responseBody}";
            //        // Log the error message
            //        Console.WriteLine(errorMessage);
            //        throw new Exception(errorMessage);
            //    }
            //}
            //else
            //{
            //    returnedUser = LoginEntraIdAsync().Result;
            //}

            return await Task.FromResult(new User { AccessToken="hello", EmailAddress="someemail"});
        }

        public async Task<User> LoginEntraIdAsync()
        {
            var returnedUser=new User();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://entraid.com/auth/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "3752392f-11da-42cc-8fd2-d1696dc642bd" },
                { "client_secret", "04d706b5-2fbf-40d1-9609-4f3f35621577" },
                { "username", "sandhya@mannu9058gmail.onmicrosoft.com" },
                { "password", "Mitom@12" }
            });

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // await _localStorage.SetItemAsync("authToken", tokenResponse.AccessToken);
                returnedUser.AuthState= new HybridAuthState { IsAuthenticated = true };
            }

            return returnedUser;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = (user.Password);
            string serializedUser = JsonConvert.SerializeObject(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RegisterUser");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public async Task<User> RefreshTokenAsync(RefreshRequest refreshRequest)
        {
            string serializedUser = JsonConvert.SerializeObject(refreshRequest);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RefreshToken");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public async Task<User> GetUserByAccessTokenAsync(string accessToken)
        {
            string serializedRefreshRequest = JsonConvert.SerializeObject(accessToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/GetUserByAccessToken");
            requestMessage.Content = new StringContent(serializedRefreshRequest);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }
    }
}
