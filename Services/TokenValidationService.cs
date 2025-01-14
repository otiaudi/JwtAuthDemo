// using System.IdentityModel.Tokens.Jwt;

// namespace JwtAuthDemo.Services
// {
//     public class TokenValidationService
//     {
//         public string GetUsernameFromAccessToken(string authorizationHeader)
//         {
//             // Check if the Authorization header is present and follows the "Bearer token" format
//             if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
//             {
//                 throw new UnauthorizedAccessException("Access token is missing or invalid.");
//             }

//             // Extract the token from the Authorization header (remove the "Bearer " part)
//             var token = authorizationHeader.Substring("Bearer ".Length).Trim();

//             if (string.IsNullOrEmpty(token))
//             {
//                 throw new UnauthorizedAccessException("Access token is missing.");
//             }

//             // Parse the JWT token and extract claims
//             var handler = new JwtSecurityTokenHandler();
//             var jwtToken = handler.ReadJwtToken(token);

//             // Retrieve the 'sub' claim which contains the username
//             var username = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

//             if (string.IsNullOrEmpty(username))
//             {
//                 throw new UnauthorizedAccessException("Invalid token. Username claim not found.");
//             }

//             return username;
//         }
//     }
// }

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Collections.Generic;
using System;

namespace JwtAuthDemo.Services
{
    public class TokenValidationService
    {
        // We will store tokens that have been refreshed here
        private static readonly HashSet<string> RefreshedTokens = new HashSet<string>();  // Track used tokens (JTI - JWT ID)

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

            // Check if the token has already been refreshed
            if (jwtToken != null && RefreshedTokens.Contains(jwtToken.Id))  // jwtToken.Id is the 'jti' claim
            {
                throw new UnauthorizedAccessException("This token has already been used to generate a new access token.");
            }

            // Track the token as refreshed after it's validated
            if (jwtToken != null)
            {
                RefreshedTokens.Add(jwtToken.Id);
            }

            return username;
        }
    }
}
