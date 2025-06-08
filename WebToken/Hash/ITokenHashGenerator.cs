namespace WebToken.Hash
{
    public interface ITokenHashGenerator
    {
        public string Hash(string input);
        public bool ValidHash(string input);
    }
}