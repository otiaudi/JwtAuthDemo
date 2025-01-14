// using JwtAuthDemo.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer; // For JWT Bearer Authentication
// using Microsoft.IdentityModel.Tokens; // For Token Validation
// using System.Text; // For Encoding.UTF8

// var builder = WebApplication.CreateBuilder(args);

// // Get the Jwt configuration values from appsettings.json
// var jwtKey = builder.Configuration["Jwt:Key"] 
//              ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured in appsettings.json.");
// var jwtIssuer = builder.Configuration["Jwt:Issuer"] 
//                 ?? throw new ArgumentNullException("Jwt:Issuer", "JWT Issuer is not configured in appsettings.json.");
// var jwtAudience = builder.Configuration["Jwt:Audience"] 
//                   ?? throw new ArgumentNullException("Jwt:Audience", "JWT Audience is not configured in appsettings.json.");

// builder.Services.AddControllers();  // Ensure this line is here to register controllers


// // Register Services with the Jwt configuration
// builder.Services.AddSingleton<TokenService>(new TokenService(jwtKey, jwtIssuer, jwtAudience));
// builder.Services.AddSingleton<RefreshTokenService>();
// builder.Services.AddSingleton<TokenValidationService>();

// builder.Logging.AddConsole();


// // Add JWT Authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtIssuer,
//             ValidAudience = jwtAudience,
//             ClockSkew = TimeSpan.Zero,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//         };
//     });

// builder.Services.AddAuthorization();

// var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();

// app.Run();


using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Get JWT configuration values
var jwtKey = builder.Configuration["Jwt:Key"] 

// Get the Jwt configuration values from appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured in appsettings.json.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException("Jwt:Issuer", "JWT Issuer is not configured in appsettings.json.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? throw new ArgumentNullException("Jwt:Audience", "JWT Audience is not configured in appsettings.json.");

// Configure TokenValidationParameters
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtIssuer,
    ValidAudience = jwtAudience,
    ClockSkew = TimeSpan.Zero, // Optional: No clock skew for testing
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
};

// Register services
builder.Services.AddSingleton(tokenValidationParameters);  // Register TokenValidationParameters for DI
builder.Services.AddControllers();  // Ensure this line is here to register controllers

// Register TokenService with the Jwt configuration

builder.Services.AddSingleton<TokenService>(new TokenService(jwtKey, jwtIssuer, jwtAudience));
builder.Services.AddSingleton<RefreshTokenService>();
builder.Services.AddSingleton<TokenValidationService>();


builder.Services.AddControllers();
builder.Logging.AddConsole();


// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = tokenValidationParameters;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };

    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
