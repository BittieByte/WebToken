using System;
using WebToken.Model;
using WebToken.Service;

namespace WebToken.Validation
{
    public class WebTokenValidator
    {
        public static ValidationResult<T> IsValid<T>(IWebTokenService tokenService, string input, params (string Key, object ExpectedValue)[] requiredClaims) where T : ITokenContainerModel
        {
            if(!tokenService.TryDecode(input, out T token)) return new ValidationResult<T> { Success = false, FailureReason = "Malformed" };
            var now = DateTimeOffset.UtcNow;

            var tokenExp = GetDateTimeClaim(token, "exp");
            var tokenNbf = GetDateTimeClaim(token, "nbf");

            if (tokenExp.HasValue && now > tokenExp.Value)
            {
                return new ValidationResult<T>
                {
                    Success = false,
                    FailureReason = "Token expired",
                    Result = token
                };
            }

            if (tokenNbf.HasValue && now < tokenNbf.Value)
            {
                return new ValidationResult<T>
                {
                    Success = false,
                    FailureReason = "Token not yet valid (nbf)",
                    Result = token
                };
            }

            // Check all required claims
            foreach (var (key, expectedValue) in requiredClaims)
            {
                if (!token.Claims.TryGetValue(key, out var actualValue))
                {
                    return new ValidationResult<T>
                    {
                        Success = false,
                        FailureReason = $"Missing claim: {key}",
                        Result = token
                    };
                }

                if (expectedValue.GetType() != typeof(SkipValueCheck)) //Use SkipValueCheck to just check if claim exists
                if (!Equals(actualValue?.ToString(), expectedValue?.ToString())) // Compare as strings for safety
                    {
                        return new ValidationResult<T>
                        {
                            Success = false,
                            FailureReason = $"Claim mismatch: {key}. Expected: {expectedValue}, Actual: {actualValue}",
                            Result = token
                        };
                }
            }

            // All checks passed
            return new ValidationResult<T> { Success = true, Result = token };
        }

        private static DateTimeOffset? GetDateTimeClaim(ITokenContainerModel token, string claimName)
        {
            if (token.Claims.TryGetValue(claimName, out var value))
            {
                if (value is long longVal)
                    return DateTimeOffset.FromUnixTimeSeconds(longVal);
                if (value is string str && long.TryParse(str, out var longParsed))
                    return DateTimeOffset.FromUnixTimeSeconds(longParsed);
            }

            return null;
        }

        public class SkipValueCheck
        {
        }

        public static readonly SkipValueCheck SkipValueCheckObject = new();
    }
}