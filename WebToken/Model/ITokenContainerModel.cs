namespace WebToken.Model
{
    public interface ITokenContainerModel
    {
        public bool IsValid(ITokenContainerModel validationParams);
    }
}