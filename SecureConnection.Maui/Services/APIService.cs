using CertificateManager;
using EncryptDecryptLib;
using Newtonsoft.Json;
using SecureConnection.DTO;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace SecureConnection.Maui.Services;

public class APIService
{
    private HttpClient _client;
    private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
    private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;
    private readonly ImportExportCertificate _importExportCertificate;
    private readonly DigitalSignatures _digitalSignatures;
    public APIService(SymmetricEncryptDecrypt symmetricEncryptDecrypt, AsymmetricEncryptDecrypt asymmetricEncryptDecrypt, ImportExportCertificate importExportCertificate, DigitalSignatures digitalSignatures)
    {
        _client = new()
        {
            BaseAddress = new Uri("https://secureconnection.azurewebsites.net"),
        };
        _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
        _asymmetricEncryptDecrypt = asymmetricEncryptDecrypt;
        _importExportCertificate = importExportCertificate;
        _digitalSignatures = digitalSignatures;
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
            return await request.Content.ReadAsStringAsync();
        return string.Empty;
    }

    private async Task<string> cryptUserDTO(UserDTO user)
    {
        var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();

        var encryptedName = _symmetricEncryptDecrypt.Encrypt(JsonConvert.SerializeObject(user.Name), IVBase64, Key);
        var encryptedPassword = _symmetricEncryptDecrypt.Encrypt(JsonConvert.SerializeObject(user.Password), IVBase64, Key);

        var targetUserPublicCertificate = GetCertificateWithPublicKeyForIdentity();


        //var encryptedKey = _asymmetricEncryptDecrypt.Encrypt(Key,
        //    Utils.CreateRsaPublicKey(targetUserPublicCertificate));

        //var encryptedIV = _asymmetricEncryptDecrypt.Encrypt(IVBase64,
        //    Utils.CreateRsaPublicKey(targetUserPublicCertificate));

        //var encryptedSender = _asymmetricEncryptDecrypt.Encrypt(user.Name,
        //    Utils.CreateRsaPublicKey(targetUserPublicCertificate));

        var cert = X509Certificate2.CreateFromEncryptedPem(targetUserPublicCertificate.PublicKey.ToString(), Key, Constants.Constants.PemPasswordExportImport);

        var encryptedKey = _asymmetricEncryptDecrypt.Encrypt(Key, cert.GetRSAPublicKey());
        var encryptedIV = _asymmetricEncryptDecrypt.Encrypt(IVBase64, cert.GetRSAPublicKey());
        var encryptedSender = _asymmetricEncryptDecrypt.Encrypt(user.Name, cert.GetRSAPublicKey());


        var certLoggedInUser = GetCertificateWithPrivateKeyForIdentity();

        var signature = _digitalSignatures.Sign(encryptedName,
            Utils.CreateRsaPrivateKey(certLoggedInUser));

        var encryptedDto = new UserDTO
        {
            Name = encryptedName,
            Password = encryptedPassword,
            Key = encryptedKey,
            IV = encryptedIV,
            DigitalSignature = signature,
            Sender = encryptedSender
        };

        return JsonConvert.SerializeObject(encryptedDto);
    }
    private X509Certificate2 GetCertificateWithPublicKeyForIdentity()
    {
        var cert = _importExportCertificate.PemImportCertificate(Constants.Constants.PemPublicKey);
        return cert;
    }

    private X509Certificate2 GetCertificateWithPrivateKeyForIdentity()
    {
        return _importExportCertificate.PemImportCertificate(Constants.Constants.PemPublicKey,
                Constants.Constants.PemPasswordExportImport);
    }
}
