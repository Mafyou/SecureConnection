﻿namespace SecureConnection.DTO;

public record RootDTO
{
    public string IV { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DigitalSignature { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
}
