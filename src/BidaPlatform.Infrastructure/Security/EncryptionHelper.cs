using System.Security.Cryptography;
using System.Text;

namespace BidaPlatform.Infrastructure.Security;

/// <summary>
/// Encryption helper for AES-256 encryption/decryption.
/// Supports random-IV encryption and deterministic encryption for searchable fields.
/// </summary>
public static class EncryptionHelper
{
    private static byte[]? _keyBytes;

    /// <summary>
    /// Configures encryption key (Base64 AES-256 key).
    /// Must be called once at application startup.
    /// </summary>
    public static void ConfigureKey(string base64Key)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
            throw new InvalidOperationException("Encryption key cannot be null or empty.");

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(base64Key);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "Security:EncryptionKey must be a valid Base64 string. " +
                "Generate a 32-byte AES key and store its Base64 value in appsettings.Secrets.json. " +
                "Example (PowerShell): [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))",
                ex);
        }

        if (keyBytes.Length != 32)
            throw new InvalidOperationException(
                "Encryption key must be 32 bytes (Base64 encoded AES-256 key).");

        _keyBytes = keyBytes;
    }

    // =====================================================
    // RANDOM IV AES (HIGH SECURITY – NON DETERMINISTIC)
    // =====================================================

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        EnsureKeyConfigured();

        using var aes = Aes.Create();
        aes.Key = _keyBytes!;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();

        // prepend IV
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        EnsureKeyConfigured();

        byte[] fullCipher;
        try
        {
            fullCipher = Convert.FromBase64String(cipherText);
        }
        catch (FormatException)
        {
            // Legacy/plain-text value stored in DB.
            return cipherText;
        }

        // AES-CBC payload is expected to contain a 16-byte IV prefix.
        if (fullCipher.Length <= 16)
            return cipherText;

        using var aes = Aes.Create();
        aes.Key = _keyBytes!;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        try
        {
            var iv = new byte[16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var cipherBytes = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);

            return sr.ReadToEnd();
        }
        catch (ArgumentException)
        {
            return cipherText;
        }
        catch (CryptographicException)
        {
            return cipherText;
        }
    }

    // =====================================================
    // DETERMINISTIC AES (SEARCHABLE – EMAIL, IDENTIFIER)
    // =====================================================

    /// <summary>
    /// Deterministic AES encryption (fixed IV).
    /// Use ONLY for searchable fields (Email).
    /// </summary>
    public static string EncryptDeterministic(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        EnsureKeyConfigured();

        using var aes = Aes.Create();
        aes.Key = _keyBytes!;
        aes.IV = new byte[16]; // fixed IV (deterministic)
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(plainText.ToLowerInvariant());
        var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        return Convert.ToBase64String(encrypted);
    }

    public static string DecryptDeterministic(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        EnsureKeyConfigured();

        using var aes = Aes.Create();
        aes.Key = _keyBytes!;
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        try
        {
            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException)
        {
            return cipherText;
        }
        catch (CryptographicException)
        {
            return cipherText;
        }
    }

    // =====================================================
    // INTERNAL
    // =====================================================

    private static void EnsureKeyConfigured()
    {
        if (_keyBytes is null)
            throw new InvalidOperationException(
                "Encryption key not configured. Call EncryptionHelper.ConfigureKey() at startup.");
    }
}
