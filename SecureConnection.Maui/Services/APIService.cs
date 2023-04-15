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
            return await request.Content.ReadAsStringAsync();
        }
        var pb = await request.Content.ReadFromJsonAsync<ProblemDetails>();
        return string.Empty;
    }
    public async Task<UserDTO> EncryptedSecureUserUnCryptedRaced(UserDTO user)
    {
        var encryptUser = cryptUserDTO(user);
        var request = await _client.PostAsJsonAsync("/user/UserUnCrypted", encryptUser);
        if (request.IsSuccessStatusCode)
        {
            return await request.Content.ReadFromJsonAsync<UserDTO>();
        }
        var pb = await request.Content.ReadFromJsonAsync<ProblemDetails>();
        throw new Exception($"{pb.Title} {pb.Detail}");
    }

    private UserDTO cryptUserDTO(UserDTO user)
    {
        var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

        return new UserDTO
        {
            Name = CipherEncrypt.EncryptCipher(user.Name, key),
            Password = CipherEncrypt.EncryptCipher(user.Password, key)
        };
    }
}
