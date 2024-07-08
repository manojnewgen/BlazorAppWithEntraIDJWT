using BlazorAppWithEntraIDJWT;
using BlazorAppWithEntraIDJWT.Components;
using BlazorAppWithEntraIDJWT.Data;
using Blazored.LocalStorage;
using BlazorServerApp.Data;
using BlazorServerApp.Handlers;
using BlazorServerApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Radzen;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add Local Storage
builder.Services.AddBlazoredLocalStorage();


builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var appSettingSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingSection);

builder.Services.AddTransient<ValidateHeaderHandler>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());



builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpClient<IUserService, UserService>();

builder.Services.AddHttpClient<IBookStoresService<Author>, BookStoresService<Author>>()
        .AddHttpMessageHandler<ValidateHeaderHandler>();
builder.Services.AddHttpClient<IBookStoresService<Publisher>, BookStoresService<Publisher>>()
        .AddHttpMessageHandler<ValidateHeaderHandler>();

builder.Services.AddSingleton<HttpClient>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SeniorEmployee", policy =>
        policy.RequireClaim("IsUserEmployedBefore1990", "true"));
});

// Configure HttpClient with HybridAuthenticationHandler

builder.Services.AddScoped(sp =>
{
    var apiSettings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
    return new HttpClient { BaseAddress = new Uri(apiSettings.BaseAddress) };
});


// Add NotificationService
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<BroweserStorageService>(provide => new BroweserStorageService(provide.GetRequiredService<IJSRuntime>()));


//Configure MSAL authentication
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
});


// Add AuthService
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();