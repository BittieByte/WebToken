using System;
using WebToken.Model;
using WebToken.Service;

namespace WebToken.Validation
{
    public class WebTokenValidator
    {
        public static ValidationResult IsValid<T>(IWebTokenService tokenService,string input,params (string Key, object ExpectedValue)[] requiredClaims) where T : ITokenContainerModel
        {
            return IsValid(tokenService, input, out T _, requiredClaims);
        }

        public static ValidationResult IsValid<T>(IWebTokenService tokenService, string input, out T token, params (string Key, object ExpectedValue)[] requiredClaims) where T : ITokenContainerModel
        {
            if(!tokenService.TryDecode(input, out token)) return new ValidationResult { IsValid = false, FailureReason = "Malformed" };
            var now = DateTimeOffset.UtcNow;

            var tokenExp = GetDateTimeClaim(token, "exp");
            var tokenNbf = GetDateTimeClaim(token, "nbf");

            if (tokenExp.HasValue && now > tokenExp.Value)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    FailureReason = "Token expired"
                };
            }

            if (tokenNbf.HasValue && now < tokenNbf.Value)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    FailureReason = "Token not yet valid (nbf)"
                };
            }

            // Check all required claims
            foreach (var (key, expectedValue) in requiredClaims)
            {
                if (!token.Claims.TryGetValue(key, out var actualValue))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        FailureReason = $"Missing claim: {key}"
                    };
                }

                // Compare as strings for safety
                if (!Equals(actualValue?.ToString(), expectedValue?.ToString()))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        FailureReason = $"Claim mismatch: {key}. Expected: {expectedValue}, Actual: {actualValue}"
                    };
                }
            }

            // All checks passed
            return new ValidationResult { IsValid = true };
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
    }

}