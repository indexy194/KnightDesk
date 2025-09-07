using System;
using System.Security.Cryptography;
using System.Text;

namespace KnightDesk.Presentation.WPF.Services
{
    /// <summary>
    /// Service for encrypting and decrypting passwords using Rijndael/AES encryption
    /// Compatible with .NET Framework 3.5
    /// </summary>
    public static class EncryptionService
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("KnightDesk2024SecureKey123456789012"); // 32 bytes for AES-256
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("KnightDesk123456"); // 16 bytes for AES IV

        /// <summary>
        /// Encrypts a password using AES encryption
        /// </summary>
        /// <param name="password">Plain text password to encrypt</param>
        /// <returns>Base64 encoded encrypted password</returns>
        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            try
            {
                using (var rijndael = new RijndaelManaged())
                {
                    rijndael.Key = key;
                    rijndael.IV = iv;
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;

                    using (var encryptor = rijndael.CreateEncryptor())
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypts a password using AES decryption
        /// </summary>
        /// <param name="encryptedPassword">Base64 encoded encrypted password</param>
        /// <returns>Decrypted plain text password</returns>
        public static string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return string.Empty;

            try
            {
                using (var rijndael = new RijndaelManaged())
                {
                    rijndael.Key = key;
                    rijndael.IV = iv;
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;

                    using (var decryptor = rijndael.CreateDecryptor())
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
