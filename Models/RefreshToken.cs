using System;
//Microsoft.EntityFrameworkCore; // For DbContext
namespace JwtAuthDemo.Models;
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public  string ? Token { get; set; } // The token itself
        public string? Username { get ; set; } // The user that the token belongs to i.e Associated 
        public DateTime ExpiryDate {get ; set ; } // The date the token expires
         public bool IsRevoked { get; set; } // Indicates if the token is revoked
    }