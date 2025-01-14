using JwtAuthDemo.Data;
using JwtAuthDemo.Models;
using System;
using System.Linq;

namespace JwtAuthDemo.Services
{
    public class RefreshTokenService
    {
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
            var refreshToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                _dbContext.SaveChanges();
            }
        }
    }
}
