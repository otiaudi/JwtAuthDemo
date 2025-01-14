using System.IdentityModel.Tokens.Jwt;

namespace JwtAuthDemo.Services
{
    public class TokenValidationService
    {
        public string GetUsernameFromAccessToken(string authorizationHeader)
        {
            // Check if the Authorization header is present and follows the "Bearer token" format
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid.");
            }

            // Extract the token from the Authorization header (remove the "Bearer " part)
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Access token is missing.");
            }

            // Parse the JWT token and extract claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Retrieve the 'sub' claim which contains the username
            var username = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Invalid token. Username claim not found.");
            }

            return username;
        }
    }
}
