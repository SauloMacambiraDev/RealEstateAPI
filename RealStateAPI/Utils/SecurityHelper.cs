using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Security.Cryptography;
using Aes = System.Security.Cryptography.Aes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Intrinsics.X86;

namespace RealStateAPI.Utils
{
    public class SecurityHelper
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] secureKey;
        private readonly byte[] iv;

        public SecurityHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            secureKey = Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"]);
            iv = Encoding.UTF8.GetBytes(_configuration["Security:IV"]);
        }
        
        public string EncryptPassword(string password) {
            if(string.IsNullOrEmpty(password)) throw new ArgumentNullException("Password can't be null or empty!");

            using(Aes aes = Aes.Create())
            {
                aes.Key = secureKey;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using(var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using(var swEncrypt = new System.IO.StreamWriter(csEncrypt)) {
                            swEncrypt.Write(password);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        public string DecryptPassword(string cipherText) {
            if (cipherText == null) throw new ArgumentNullException("CipherText can't be null");
            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = secureKey;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

        }

        public void CreateHashPassword(string password, out byte[] hash, out byte[] salt)
        {
            if (password == null) throw new ArgumentNullException("Please, provide some value for password");
            
            using(var hmac = new HMACSHA256())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
            }
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("Please, provide some value for password");

            using (var hmac = new HMACSHA256(storedSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
