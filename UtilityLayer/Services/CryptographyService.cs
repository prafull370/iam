using System.Security.Cryptography;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Configuration;
using UtilityLayer.Interfaces;

namespace UtilityLayer.Services
{
    public class CryptographyService:ICryptographyService
    {
        private readonly IConfiguration config;
         public CryptographyService(IConfiguration configuration)
         {
            config = configuration;    
         }
        public string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(config["Cryptography:Key"]);
            byte[] ivBytes = Encoding.UTF8.GetBytes(config["Cryptography:Iv"]);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            byte[] keyBytes = Encoding.UTF8.GetBytes(config["Cryptography:Key"]);
            byte[] ivBytes = Encoding.UTF8.GetBytes(config["Cryptography:Iv"]);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
