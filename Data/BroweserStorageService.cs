using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorAppWithEntraIDJWT.Data
{
    public class BroweserStorageService
    {
        private const string StorageType= "localStorage";
        private readonly IJSRuntime _Jsruntime;

    
        public BroweserStorageService(IJSRuntime  jSRuntime)
        {
              _Jsruntime = jSRuntime;
        }

        public async Task SaveToStorage<TData>(string key, TData data)
        {
            var serializedData = Serialize(data);
            await _Jsruntime.InvokeVoidAsync($"{StorageType}.setItem", key, serializedData);
        }

        public async Task<TData> GetFromStorage<TData>(string key)
        {
            var serializedData = await _Jsruntime.InvokeAsync<string>($"{StorageType}.getItem", key);
            return Deserialize<TData>(serializedData);
        }

        public async Task RemoveFromStorage(string key)
        {
            await _Jsruntime.InvokeVoidAsync($"{StorageType}.removeItem", key);
        }

        public static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false
        };
        public static string Serialize<TData>(TData data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data, _jsonSerializerOptions);
        }

        public static TData Deserialize<TData>(string data)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TData>(data);
        }
    }
}
