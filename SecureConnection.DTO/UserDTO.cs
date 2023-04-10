namespace SecureConnection.DTO;

public sealed record UserDTO : RootDTO
{
    public string Name { get; set; }
    public string Password { get; set; }
}