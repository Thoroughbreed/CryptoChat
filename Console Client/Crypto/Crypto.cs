using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Console_Client.Crypto
{
    public class CryptoClass
    {
        // The default unlock-key for en-/decryption of messages
        private string _masterKey = "";
        // The default i-Vector for en-/decryption of the encrypted List<Password>
        private string _iv = "E8B48407387C429B681C7BE4211D1860";

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="length">Length in bytes</param>
        /// <returns>Random number</returns>
        private byte[] GenerateRandomNumber(int length)
        {
            byte[] randomNumber = new byte[length];
            RandomNumberGenerator.Create().GetBytes(randomNumber);
            return randomNumber;
        }

        /// <summary>
        /// This is where the magic happens
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="encrypt">Encrypt/decrypt (default decrypt)</param>
        /// <returns>Byte of the en/decrypted data</returns>
        private byte[] Crypto(byte[] data, bool encrypt = false)
        {
            using var cryptography = Aes.Create();
            cryptography.Key = Convert.FromHexString(_masterKey);
            if (encrypt)
            {
                return cryptography.EncryptCbc(data, Convert.FromHexString(_iv));
            }
            else
            {
                return cryptography.DecryptCbc(data, Convert.FromHexString(_iv));
            }
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="data">String to be encrypted</param>
        /// <returns>Cipher text</returns>
        public string Encrypt(string data)
        {
            return Convert.ToHexString(Crypto(Encoding.UTF8.GetBytes(data), true));
        }

        /// <summary>
        /// Decrypt a cipher text
        /// </summary>
        /// <param name="text">Cipher text</param>
        /// <returns>Decrypted data</returns>
        public string Decrypt(byte[] cipher)
        {
            return Encoding.UTF8.GetString(Crypto(cipher));
        }

        /// <summary>
        /// Gets the master-password and sets the unlock key
        /// </summary>
        /// <param name="unlock">Master-password</param>
        public string SetUnlockKey(string unlock)
        {
            var bytes = Encoding.UTF8.GetBytes(unlock);
            using SHA256 sha256 = SHA256.Create();
            _masterKey = Convert.ToHexString(sha256.ComputeHash(bytes));
            return unlock;
        }
    }
}