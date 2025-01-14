
using Microsoft.IdentityModel.Tokens;  //provide tools for creating and validating tokens
using System.IdentityModel.Tokens.Jwt; // contains functionality for creating and handling JWT 
using System.Security.Claims;     //    Used to define claims, whhich are key-value pairs representing  user related information                                                                                                                                                                                                                                                                                                                                                                                 
using System.Text; //for encoding and decoding text-converting strings to bytes

namespace JwtAuthDemo.Services
{
    public class TokenService
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenService(string jwtKey, string jwtIssuer, string jwtAudience)
        {
            _jwtKey = jwtKey;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,     
                audience: _jwtAudience,
                claims: claims,

                // expires: DateTime.Now.AddHours(1),
                expires: DateTime.Now.AddMinutes(1), // For testing purposes

                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
