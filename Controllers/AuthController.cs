using Microsoft.AspNetCore.Mvc;
using JwtAuthDemo.Services; 
using Microsoft.AspNetCore.Authorization;


namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Simulate a valid user (you can replace this with actual authentication logic)
            var username = "testuser";

            // Generate JWT Token using TokenService
            var token = _tokenService.GenerateToken(username);

            // Return the token
            return Ok(new { Token = token });
        }

        
        // Protected endpoint to test the JWT token
        [Authorize]
        [HttpGet("protected-data")]
         // This ensures the endpoint requires a valid JWT token
        public IActionResult GetProtectedData()
        {
            return Ok(new { Message = "This is protected data. You are authorized!" });
        }
    }
    }

