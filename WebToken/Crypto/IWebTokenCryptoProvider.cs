namespace WebToken.Crypto
{
    public interface IWebTokenCryptoProvider
    {
        public byte[] Encrypt(byte[] input);
        public byte[] Decrypt(byte[] input);
        public byte[] GetBytes();
    }
}