using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using RentIt.Shared.Abstractions.Security;

namespace RentIt.Shared.Infrastructure.Security;

public class AesGcmEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesGcmEncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["AESGCM:KEY"];
        if (string.IsNullOrEmpty(keyString))
        {
            throw new InvalidOperationException("AESGCM:KEY is not configured.");
        }

        _key = Encoding.UTF8.GetBytes(keyString);
        
        if (_key.Length != 32)
        {
            throw new InvalidOperationException($"AES-256 requires a 32-byte key, but the configured key is {_key.Length} bytes.");
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherText = new byte[plainTextBytes.Length];
        byte[] nonce = new byte[12]; // 12 bytes is the standard nonce size for GCM
        RandomNumberGenerator.Fill(nonce);
        byte[] tag = new byte[16]; // 16 bytes is standard for GCM authentication tag

        using var aesGcm = new AesGcm(_key, tag.Length);
        aesGcm.Encrypt(nonce, plainTextBytes, cipherText, tag);

        // Combine Nonce + Tag + CipherText
        byte[] encryptedData = new byte[nonce.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, encryptedData, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, encryptedData, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, encryptedData, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string cipherTextString)
    {
        if (string.IsNullOrEmpty(cipherTextString))
        {
            return cipherTextString;
        }

        byte[] encryptedData = Convert.FromBase64String(cipherTextString);

        byte[] nonce = new byte[12];
        byte[] tag = new byte[16];
        byte[] cipherText = new byte[encryptedData.Length - nonce.Length - tag.Length];

        Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(encryptedData, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(encryptedData, nonce.Length + tag.Length, cipherText, 0, cipherText.Length);

        byte[] plainTextBytes = new byte[cipherText.Length];

        using var aesGcm = new AesGcm(_key, tag.Length);
        aesGcm.Decrypt(nonce, cipherText, tag, plainTextBytes);

        return Encoding.UTF8.GetString(plainTextBytes);
    }
}
