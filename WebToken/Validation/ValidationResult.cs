using WebToken.Model;

namespace WebToken.Validation
{
    public class ValidationResult<T> where T : ITokenContainerModel
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        public T Result { get; set; } 

        public override string ToString()
        {
            return Success ? "Success" : FailureReason;
        }
    }
}