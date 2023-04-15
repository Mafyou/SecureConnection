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
                return Ok((user.Name == "Mafyou") && (user.Password == "test"));
            }
            catch (Exception ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message, Detail = ex.StackTrace });
            }
        }
        [HttpPost("UserUnCrypted")]
        [AllowAnonymous]
        public IActionResult UserUnCrypted(UserDTO userDTO)
        {
            try
            {
                return Ok(decryptUserDTO(userDTO));
            }
            catch (Exception ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message, Detail = ex.StackTrace });
            }
        }

        private UserDTO decryptUserDTO(UserDTO userEncrypted)
        {
            var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            return new UserDTO
            {
                Name = CipherDecrypt.DecryptCipher(userEncrypted.Name, key),
                Password = CipherDecrypt.DecryptCipher(userEncrypted.Password, key)
            };
        }
    }
}