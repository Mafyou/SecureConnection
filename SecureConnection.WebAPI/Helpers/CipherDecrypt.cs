namespace SecureConnection.WebAPI.Helpers;

public static class CipherDecrypt
{
    public static string DecryptCipher(string toDecrypt, byte[] key)
    {
        var fullCipher = Convert.FromBase64String(toDecrypt);
        var iv = new byte[16];
        var cipher = new byte[16];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
        var result = string.Empty;
        using (var aesCryptor = Aes.Create())
        using (var decrypt = aesCryptor.CreateDecryptor(key, iv))
        using (var msDecrypt = new MemoryStream(cipher))
        using (var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
            result = srDecrypt.ReadToEnd();
        return result;
    }
}