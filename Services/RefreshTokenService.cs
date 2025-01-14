
using Microsoft.Extensions.Logging;

using JwtAuthDemo.Data;
using JwtAuthDemo.Models;
using System;
using System.Linq;


namespace JwtAuthDemo.Services
{
    public class RefreshTokenService
    {

        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        private readonly ILogger<RefreshTokenService> _logger;

         public RefreshTokenService(ILogger<RefreshTokenService> logger)
            {
                _logger = logger;
            }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                          .Replace("=", "")
                          .Replace("+", "")
                          .Replace("/", "");
        }

        public RefreshToken CreateRefreshToken(string username)
        {
            var oldToken = _refreshTokens.FirstOrDefault(t => t.Username == username && !t.IsRevoked);
            if (oldToken != null)
            {
                // if (oldToken != null)
                // {
                    if (oldToken.Token != null)
                    {
                        RevokeRefreshToken(oldToken.Token);  // Revoke the old refresh token
                    }
                // }
            }
            
            var token = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Username = username,
                // ExpiryDate = DateTime.UtcNow.AddDays(7),
                ExpiryDate = GetRefreshTokenExpiry(),
                IsRevoked = false
            };

            _refreshTokens.Add(token);
            _logger.LogInformation("Created and stored refresh token for user {Username}: {Token}", username, token.Token);
            return token;
        }

         public RefreshToken GetRefreshToken(string username)
        {
            return _refreshTokens.FirstOrDefault(t => t.Username == username && !t.IsRevoked)!;
        }

        
          public bool ValidateRefreshToken(string token)
        {
            var storedToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
           
            if (storedToken == null)
                {
                    // Log all stored tokens for inspection
                    _logger.LogWarning("Current refresh token list: {Tokens}", 
                        string.Join(", ", _refreshTokens.Select(rt => $"[Token: {rt.Token}, Username: {rt.Username}, Expiry: {rt.ExpiryDate}, Revoked: {rt.IsRevoked}]")));
                    _logger.LogWarning("Refresh token not found: {Token}", token);
                    return false;
                }

                // Check if token is revoked or expired
                if (storedToken.IsRevoked || storedToken.ExpiryDate <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token is revoked or expired: {Token}", token);
                    return false;
                }

                _logger.LogInformation("Refresh token is valid: {Token}", token);
                return true;

        private readonly AppDbContext _dbContext;

        public RefreshTokenService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public RefreshToken GenerateRefreshToken(string username)
        {
            var token = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(), // Generate a unique token
                Username = username,
                ExpiryDate = DateTime.Now.AddDays(7), // Set expiration to 7 days
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(token);
            _dbContext.SaveChanges();

            return token;
        }

        public bool ValidateRefreshToken(string token)
        {
            var refreshToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.Token == token);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.Now)
            {
                return false;
            }

            return true;

        }

        public void RevokeRefreshToken(string token)
        {

            var storedToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
            }
        }
        

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddDays(7); 
           
        }


            var refreshToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                _dbContext.SaveChanges();
            }
        }

    }
}
