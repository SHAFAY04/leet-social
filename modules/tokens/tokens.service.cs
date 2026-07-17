using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


public class TokensService : ITokensService
{
    public GenerateTokensResponse GenerateTokens(Guid userId, string username, string email, RolesEnum role)
    {
        string accessTokenSecret = Environment.GetEnvironmentVariable("ACCESS_SECRET")!;
        string refreshTokenSecret = Environment.GetEnvironmentVariable("REFRESH_SECRET")!;

        var tokenHandler = new JwtSecurityTokenHandler();

        // Convert your Base64 or plain text secret string into raw bytes
        byte[] accessKeyBytes = Encoding.UTF8.GetBytes(accessTokenSecret!);
        var accessSigningKey = new SymmetricSecurityKey(accessKeyBytes);
        byte[] refreshKeyBytes = Encoding.UTF8.GetBytes(refreshTokenSecret!);
        var refreshSigningKey = new SymmetricSecurityKey(refreshKeyBytes);

        //payload
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name,username),
            new Claim(ClaimTypes.Role,role.ToString())
        };

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(accessSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var refreshTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(refreshSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken accessTokenObject = tokenHandler.CreateToken(accessTokenDescriptor);
        SecurityToken refreshTokenObject = tokenHandler.CreateToken(refreshTokenDescriptor);
        string accessTokenString = tokenHandler.WriteToken(accessTokenObject);
        string refreshTokenString = tokenHandler.WriteToken(refreshTokenObject);

        return new GenerateTokensResponse(accessToken: accessTokenString, refreshToken: refreshTokenString);
    }

    public GenerateAccessTokenResponse GenerateAccessToken(string userId, string username, string email, string role)
    {

        var accessSecret = Environment.GetEnvironmentVariable("ACCESS_SECRET");

        var tokenHandler = new JwtSecurityTokenHandler();

        byte[] accessKeyBytes = Encoding.UTF8.GetBytes(accessSecret);
        var accessSigningKey = new SymmetricSecurityKey(accessKeyBytes);

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name,username),
            new Claim(ClaimTypes.Email,email),
            new Claim(ClaimTypes.Role,role)

        };

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(accessSigningKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenObject = tokenHandler.CreateToken(securityTokenDescriptor);
        string accessToken = tokenHandler.WriteToken(tokenObject);

        return new GenerateAccessTokenResponse(accessToken);

    }

    public RefreshTheAccessTokenResponse RefreshTheAccessToken(string refreshToken)
    {
        var refreshSecret = Environment.GetEnvironmentVariable("REFRESH_SECRET");
        var tokenHandler = new JwtSecurityTokenHandler();


        var principle = tokenHandler.ValidateToken(
           refreshToken,
           new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(refreshSecret)
               ),
               ClockSkew = TimeSpan.Zero
           },
           out SecurityToken validatedToken
       );
        var email = principle.FindFirst(ClaimTypes.Email)?.Value;
        var username = principle.FindFirst(ClaimTypes.Name)?.Value;
        var userid = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = principle.FindFirst(ClaimTypes.Role)?.Value;

        var response = GenerateAccessToken(userid, username, email,role);

        return new RefreshTheAccessTokenResponse(response.accessToken);

    }
}