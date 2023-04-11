namespace SecureConnection.Maui.Helpers;

public static class CipherEncrypt
{
    public static string EncryptCipher(string toEncrypt, byte[] key)
    {
        var cryptedResult = string.Empty;

        using (var aesCryptor = Aes.Create())
        using (var encryptor = aesCryptor.CreateEncryptor(key, aesCryptor.IV))
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
                swEncrypt.Write(toEncrypt);
            var iv = aesCryptor.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            cryptedResult = Convert.ToBase64String(result);
        }

        return cryptedResult;
    }
}
