using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureConnection.DTO;
using System.Security.Cryptography;
using System.Text;

namespace SecureConnection.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost("AuthentificationCrypted")]
        [AllowAnonymous]
        public IActionResult UserAuthentification(UserDTO userDTO)
        {
            try
            {
                var user = decryptUserDTO(userDTO);
                return Ok(user);
                // return Ok((user.Name == "Mafyou") && (user.Password == "test"));
            }
            catch (Exception ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message, Detail = ex.StackTrace });
            }
        }

        private UserDTO decryptUserDTO(UserDTO userEncrypted)
        {
            var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            var nameCipher = Convert.FromBase64String(userEncrypted.Name);
            var ivName = new byte[16];
            var cipherName = new byte[16];
            Buffer.BlockCopy(nameCipher, 0, ivName, 0, ivName.Length);
            Buffer.BlockCopy(nameCipher, ivName.Length, cipherName, 0, ivName.Length);
            var name = string.Empty;
            using (var aesCryptor = Aes.Create())
            using (var decryptor = aesCryptor.CreateDecryptor(key, ivName))
            using (var msDecrypt = new MemoryStream(cipherName))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
                name = srDecrypt.ReadToEnd();

            var password = string.Empty;
            var passwordCipher = Convert.FromBase64String(userEncrypted.Password);
            var ivPassword = new byte[16];
            var cipherPassword = new byte[16];
            Buffer.BlockCopy(passwordCipher, 0, ivPassword, 0, ivPassword.Length);
            Buffer.BlockCopy(passwordCipher, ivPassword.Length, cipherPassword, 0, ivPassword.Length);
            using (var aesCryptor = Aes.Create())
            using (var decryptor = aesCryptor.CreateDecryptor(key, ivPassword))
            using (var msDecrypt = new MemoryStream(cipherPassword))
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
}