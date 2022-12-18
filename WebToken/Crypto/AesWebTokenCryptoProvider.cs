using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace WebToken.Crypto
{
    public class AesWebTokenCryptoProvider : IWebTokenCryptoProvider
    {
        private readonly byte[] _encryptionKey;
        private readonly byte[] _encryptionIv;

        public AesWebTokenCryptoProvider(byte[] encryptionKey, byte[] encryptionIv)
        {
            _encryptionKey = encryptionKey;
            _encryptionIv = encryptionIv;
        }

        public AesWebTokenCryptoProvider(string encryptionKey, string encryptionIv)
        {
            _encryptionKey = Convert.FromBase64String(encryptionKey);
            _encryptionIv = Convert.FromBase64String(encryptionIv);
        }

        public byte[] Encrypt(byte[] input)
        {
            try
            {

                using var crypto = new AesCryptoServiceProvider();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, crypto.CreateEncryptor(_encryptionKey, _encryptionIv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public byte[] Decrypt(byte[] input)
        {
            try
            {
                using var crypto = new AesCryptoServiceProvider();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, crypto.CreateDecryptor(_encryptionKey, _encryptionIv), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public byte[] GetBytes()
        {
            return _encryptionKey.Concat(_encryptionIv).ToArray();
        }
    }
}