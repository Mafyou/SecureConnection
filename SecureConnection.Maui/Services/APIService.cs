using Microsoft.AspNetCore.Mvc;
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
        var request = await _client.PostAsJsonAsync("/user/AuthentificationCrypted", encryptUser);
        var response = request.EnsureSuccessStatusCode();
        if (request.IsSuccessStatusCode)
            return await request.Content.ReadAsStringAsync();
        var pb = await request.Content.ReadFromJsonAsync<ProblemDetails>();
        return string.Empty;
    }

    private UserDTO cryptUserDTO(UserDTO user)
    {
        using var aesCryptor = Aes.Create();
        aesCryptor.Mode = CipherMode.CBC;
        aesCryptor.Padding = PaddingMode.PKCS7;
        var encryptor = aesCryptor.CreateEncryptor(aesCryptor.Key, aesCryptor.IV);

        var cryptedName = string.Empty;
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(user.Name);
            cryptedName = Encoding.UTF8.GetString(msEncrypt.ToArray());
        }
        var cryptedPassword = string.Empty;
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(user.Password);
            cryptedPassword = Encoding.UTF8.GetString(msEncrypt.ToArray());
        }
        return new UserDTO
        {
            Name = cryptedName,
            Password = cryptedPassword
        };
    }
}
