using System;

namespace WebToken.Model
{
    public static class TokenContainerModelExtensions
    {
        public static bool TryGetClaim<T>(this ITokenContainerModel token, string key, out T result)
        {
            result = default;

            if (token.Claims != null && token.Claims.TryGetValue(key, out var value))
            {
                try
                {
                    if (value is T tValue)
                    {
                        result = tValue;
                        return true;
                    }

                    if (typeof(T) == typeof(DateTimeOffset))
                    {
                        if (value is long unixTime)
                        {
                            result = (T)(object)DateTimeOffset.FromUnixTimeSeconds(unixTime);
                            return true;
                        }
                        if (long.TryParse(value?.ToString(), out var unixParsed))
                        {
                            result = (T)(object)DateTimeOffset.FromUnixTimeSeconds(unixParsed);
                            return true;
                        }
                    }

                    // Generic type conversion for primitives / strings
                    if (value != null)
                    {
                        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                        result = (T)Convert.ChangeType(value, targetType);
                        return true;
                    }
                }
                catch
                {
                    // Swallow conversion error, return false
                }
            }
            return false;
        }
    }
}