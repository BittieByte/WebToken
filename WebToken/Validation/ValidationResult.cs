namespace WebToken.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string FailureReason { get; set; } = string.Empty;

        public override string ToString()
        {
            return IsValid ? "Success" : FailureReason;
        }
    }
}