using SecureConnection.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

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
    public async Task<string> ClearSecureUserAuthentification(UserDTO user)
    {
        var request = await _client.PostAsJsonAsync("/userClearAuthentification", user);
        if (request.IsSuccessStatusCode)
            return await request.Content.ReadAsStringAsync();
        return string.Empty;
    }
    public async Task<string> EncryptedSecureUserAuthentification(UserDTO user)
    {
        var encryptUser = cryptUserDTO(user);

        using var content = new MultipartFormDataContent();
        var byteContent = new ByteArrayContent(encryptUser);
        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Plain);
        content.Add(byteContent, "file", $"{Guid.NewGuid()}.txt");
        content.Headers.Add("Id", 1.ToString());

        var request = await _client.PostAsync("/encryptedUser", content);
        var response = request.EnsureSuccessStatusCode();
        var sd = await response.Content.ReadAsStringAsync();
        if (request.IsSuccessStatusCode)
            return await request.Content.ReadAsStringAsync();
        return string.Empty;
    }

    private byte[] cryptUserDTO(UserDTO user)
    {
        using var aesCryptor = Aes.Create();
        var encryptor = aesCryptor.CreateEncryptor(aesCryptor.Key, aesCryptor.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(user);
        return ms.ToArray();
    }
}
