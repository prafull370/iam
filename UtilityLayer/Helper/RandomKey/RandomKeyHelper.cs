using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Helper.RandomKey
{
    public static class RandomKeyHelper
    {
        #region Get Random Number
        public static string GetRandomKey(int len)
        {
            int maxSize = len;
            char[] chars = new char[30];
            string a;
            a = "1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data) { result.Append(chars[b % (chars.Length)]); }
            return result.ToString();
        }
        #endregion
       public static string GeneratePassword()
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string number = "0123456789";
            const string special = "!@#$%^&*";
            const string all = upper + lower + number + special;

            Random rnd = new Random();

            // Ensure required characters
            char[] password = new char[8];
            password[0] = upper[rnd.Next(upper.Length)];
            password[1] = lower[rnd.Next(lower.Length)];
            password[2] = number[rnd.Next(number.Length)];
            password[3] = special[rnd.Next(special.Length)];

            // Fill remaining with unique chars
            int index = 4;
            while (index < 8)
            {
                char c = all[rnd.Next(all.Length)];

                if (!password.Contains(c)) // ensure unique
                {
                    password[index++] = c;
                }
            }

            // Shuffle password
            password = password.OrderBy(x => rnd.Next()).ToArray();

            return new string(password);
        }
    }
}
