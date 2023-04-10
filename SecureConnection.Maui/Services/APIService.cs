using SecureConnection.DTO;
using System.Net.Http.Json;

namespace SecureConnection.Maui.Services;

public class APIService
{
    private HttpClient _client;
    public APIService()
    {
        _client = new()
        {
            BaseAddress = new Uri("https://secureconnection.azurewebsites.net"),
        };
    }
    public async Task<string> SecureUserAuthentification(UserDTO user)
    {
        var request = await _client.PostAsJsonAsync("/userAuthentification", user);
        if (request.IsSuccessStatusCode)
            return await request.Content.ReadAsStringAsync();
        return string.Empty;
    }
}
