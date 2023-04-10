using System.Net.Http.Headers;

namespace SecureConnection.Maui.Services;

internal class APIService
{
    private HttpClient _client;
    public APIService()
    {
        _client = new()
        {
            BaseAddress = new Uri("https://secureconnection.azurewebsites.net"),
        };
    }
}
