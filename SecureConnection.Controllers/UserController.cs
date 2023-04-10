using CertificateManager;
using EncryptDecryptLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureConnection.DTO;
using System.Security.Cryptography.X509Certificates;

namespace SecureConnection.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
    private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;
    private readonly ImportExportCertificate _importExportCertificate;
    private readonly DigitalSignatures _digitalSignatures;
    public UserController(SymmetricEncryptDecrypt symmetricEncryptDecrypt, AsymmetricEncryptDecrypt asymmetricEncryptDecrypt, ImportExportCertificate importExportCertificate, DigitalSignatures digitalSignatures)
    {
        _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
        _asymmetricEncryptDecrypt = asymmetricEncryptDecrypt;
        _importExportCertificate = importExportCertificate;
        _digitalSignatures = digitalSignatures;
    }
    [HttpPost("AuthentificationCrypted")]
    [AllowAnonymous]
    public async Task<bool> UserAuthentification(UserDTO userDTO)
    {
        var user = decryptUserDTO(userDTO);
        return (user.Name == "Mafyou") && (user.Password == "test");
    }
    private UserDTO decryptUserDTO(UserDTO userEncrypted)
    {
        var cert = getCertificateWithPrivateKeyForIdentity();

        var sender = _asymmetricEncryptDecrypt.Decrypt(userEncrypted.Sender,
           Utils.CreateRsaPrivateKey(cert));

        var senderCert = getCertificateWithPublicKeyForIdentity();

        var verified = _digitalSignatures.Verify(userEncrypted.Name,
            userEncrypted.DigitalSignature,
            Utils.CreateRsaPublicKey(senderCert));

        var key = _asymmetricEncryptDecrypt.Decrypt(userEncrypted.Key,
           Utils.CreateRsaPrivateKey(cert));

        var IV = _asymmetricEncryptDecrypt.Decrypt(userEncrypted.IV, Utils.CreateRsaPrivateKey(cert));

        return new UserDTO
        {
            Name = _symmetricEncryptDecrypt.Decrypt(userEncrypted.Name, IV, key),
            Password = _symmetricEncryptDecrypt.Decrypt(userEncrypted.Password, IV, key)
        };
    }

    private X509Certificate2 getCertificateWithPublicKeyForIdentity()
    {
        var cert = _importExportCertificate.PemImportCertificate(Constants.Constants.PemPublicKey);
        return cert;
    }

    private X509Certificate2 getCertificateWithPrivateKeyForIdentity()
    {
        return _importExportCertificate.PemImportCertificate(Constants.Constants.PemPublicKey,
                Constants.Constants.PemPasswordExportImport);
    }
}