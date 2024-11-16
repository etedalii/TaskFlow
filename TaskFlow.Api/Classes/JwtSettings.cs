namespace TaskFlow.Api.Classes;

public class JwtSettings
{
    public string Secret { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int ExpirationInMinutes { get; set; } = 60; // Default to 60 minutes if not set in config
}