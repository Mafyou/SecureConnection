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
        if (request.IsSuccessStatusCode)
        {
            var lol = await request.Content.ReadFromJsonAsync<UserDTO>();
            return "toto";
        }
        var pb = await request.Content.ReadFromJsonAsync<ProblemDetails>();
        return string.Empty;
    }

    private UserDTO cryptUserDTO(UserDTO user)
    {
        var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");
        var cryptedName = string.Empty;

        using (var aesCryptor = Aes.Create())
        using (var encryptor = aesCryptor.CreateEncryptor(key, aesCryptor.IV))
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(user.Name);
            var iv = aesCryptor.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            cryptedName = Convert.ToBase64String(result);
        }
        var cryptedPassword = string.Empty;
        using (var aesCryptor = Aes.Create())
        using (var encryptor = aesCryptor.CreateEncryptor(key, aesCryptor.IV))
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(user.Password);
            var iv = aesCryptor.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            cryptedPassword = Convert.ToBase64String(result);
        }
        return new UserDTO
        {
            Name = cryptedName,
            Password = cryptedPassword
        };
    }
}
