
using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // For JWT Bearer Authentication
using Microsoft.IdentityModel.Tokens; // For Token Validation
using System.Text; // For Encoding.UTF8

var builder = WebApplication.CreateBuilder(args);

// Get the Jwt configuration values from appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"] 
             ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured in appsettings.json.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] 
                ?? throw new ArgumentNullException("Jwt:Issuer", "JWT Issuer is not configured in appsettings.json.");
var jwtAudience = builder.Configuration["Jwt:Audience"] 
                  ?? throw new ArgumentNullException("Jwt:Audience", "JWT Audience is not configured in appsettings.json.");

builder.Services.AddControllers();  // Ensure this line is here to register controllers


// Register TokenService with the Jwt configuration
builder.Services.AddSingleton<TokenService>(new TokenService(jwtKey, jwtIssuer, jwtAudience));


// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


