using System.Numerics;

namespace WebApplication1;

public class UserProfile
{
    public string? bio {get;set;}
    public string? pronouns {get; set;}
    public string? avatarUrl {get;set;}
    public string username {get;set;}
    public string email{get;set;}
}

public record UserAuthResponseDto(string accessToken,  string refreshToken, UserProfile profile );
public record UserAuthRequest(string email, string password);

public record RefreshAuthRequest(string refreshToken);

public record RefreshAuthResponseDto(string accessToken);