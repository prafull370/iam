using Microsoft.Extensions.Configuration;

namespace UtilityLayer.Interfaces
{
    public interface ICryptographyService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
