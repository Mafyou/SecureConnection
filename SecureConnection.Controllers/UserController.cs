using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureConnection.DTO;
using System.Security.Cryptography;
using System.Text;

namespace SecureConnection.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
    }
    [HttpPost("AuthentificationCrypted")]
    [AllowAnonymous]
    public IActionResult UserAuthentification(UserDTO userDTO)
    {
        try
        {
            var user = decryptUserDTO(userDTO);
            return Ok((user.Name == "Mafyou") && (user.Password == "test"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message, Detail = ex.StackTrace });
        }
    }

    private UserDTO decryptUserDTO(UserDTO userEncrypted)
    {
        using var aesCryptor = Aes.Create();
        aesCryptor.Mode = CipherMode.CBC;
        aesCryptor.Padding = PaddingMode.PKCS7;
        var decryptor = aesCryptor.CreateDecryptor(aesCryptor.Key, aesCryptor.IV);

        var cryptedName = Encoding.ASCII.GetBytes(userEncrypted.Name);
        var name = string.Empty;
        using (MemoryStream msDecrypt = new MemoryStream(cryptedName))
        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
            name = srDecrypt.ReadToEnd();

        var cryptedPassword = Encoding.ASCII.GetBytes(userEncrypted.Password);
        var password = string.Empty;
        using (MemoryStream msDecrypt = new MemoryStream(cryptedPassword))
        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
            password = srDecrypt.ReadToEnd();

        return new UserDTO
        {
            Name = name,
            Password = password
        };
    }
}