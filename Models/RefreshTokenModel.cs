public class RefreshToken
{
    public string? Token { get; set; }
    public string? Username { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
}
