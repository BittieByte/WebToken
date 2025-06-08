using System;

namespace WebToken.Model
{
    public class WebTokenBuilder<T> where T : ITokenContainerModel, new()
    {
        private readonly T _token = new();

        public WebTokenBuilder<T> WithClaim(string key, object value)
        {
            _token.Claims[key] = value;
            return this;
        }

        public WebTokenBuilder<T> WithExpiration(TimeSpan expirationSpan)
        {
            var expiration = DateTimeOffset.UtcNow + expirationSpan;
            _token.Claims["exp"] = expiration.ToUnixTimeSeconds();
            return this;
        }

        public WebTokenBuilder<T> WithExpiration(DateTimeOffset expiration)
        {
            _token.Claims["exp"] = expiration.ToUnixTimeSeconds();
            return this;
        }

        public WebTokenBuilder<T> WithNotBefore(DateTimeOffset notBefore)
        {
            _token.Claims["nbf"] = notBefore.ToUnixTimeSeconds();
            return this;
        }

        public WebTokenBuilder<T> WithNotBefore(TimeSpan expirationSpan)
        {
            var expiration = DateTimeOffset.UtcNow + expirationSpan;
            _token.Claims["nbf"] = expiration.ToUnixTimeSeconds();
            return this;
        }

        public WebTokenBuilder<T> WithIssuedAt(DateTimeOffset issuedAt)
        {
            _token.Claims["iat"] = issuedAt.ToUnixTimeSeconds();
            return this;
        }

        public WebTokenBuilder<T> WithIssuer(string issuer)
        {
            _token.Claims["iss"] = issuer;
            return this;
        }

        public WebTokenBuilder<T> WithAudience(string audience)
        {
            _token.Claims["aud"] = audience;
            return this;
        }

        public WebTokenBuilder<T> WithSubject(string subject)
        {
            _token.Claims["sub"] = subject;
            return this;
        }

        public T Build()
        {
            return _token;
        }
    }
}