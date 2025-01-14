using Microsoft.AspNetCore.Mvc;
using JwtAuthDemo.Services; 
using Microsoft.AspNetCore.Authorization;
using YourNamespace.Models;


namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly TokenValidationService _tokenValidationService;
        

        public AuthController(TokenService tokenService, RefreshTokenService refreshTokenService, TokenValidationService tokenValidationService)
        {
            _tokenService = tokenService;
            _refreshTokenService =  refreshTokenService;
             _tokenValidationService = tokenValidationService ?? throw new ArgumentNullException(nameof(tokenValidationService));  // Ensure proper injection
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Simulate a valid user (you can replace this with actual authentication logic)
            var username = "testuser";

            // Generate JWT Token using TokenService
            // var token = _tokenService.GenerateToken(username);

            var accessToken = _tokenService.GenerateToken(username);
            var refreshToken = _refreshTokenService.CreateRefreshToken(username);

            // Return the token
            return Ok(new 
            {
                //  Token = token 
                AccessToken = accessToken, 
                RefreshToken = refreshToken, 
                RefreshTokenExpiry = _refreshTokenService.GetRefreshTokenExpiry() 
                 
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Validate refresh token
                if (!_refreshTokenService.ValidateRefreshToken(request.RefreshToken))
                {
                    return Unauthorized(new { Message = "Invalid or expired refresh token." });
                }

                // Extract username from the access token (provided with the request)
                if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized(new { Message = "Authorization header is missing." });
                }

                var username = _tokenValidationService.GetUsernameFromAccessToken(authorizationHeader.ToString());

                // Generate new access token and refresh token
                var newAccessToken = _tokenService.GenerateToken(username);
                var newRefreshToken = _refreshTokenService.CreateRefreshToken(username);

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpiry = newRefreshToken.ExpiryDate
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        // Protected endpoint to test the JWT token
        [Authorize]
        [HttpGet("protected-data")]
         // This ensures the endpoint requires a valid JWT token
        public IActionResult GetProtectedData()
        {
             if (!User.Identity?.IsAuthenticated ?? false)
                {
                    return Unauthorized(new { Message = "You are not authorized to access this data." });
                }
            return Ok(new { Message = "This is protected data. You are authorized!" });
        }
    }
}

