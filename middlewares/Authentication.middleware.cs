

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Validation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

public class AuthenticationMiddleware
{
    
    RequestDelegate _next;
    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next=next;
    }

    public async Task Invoke(HttpContext context)
    {
        
        var accessTokenSecret= Environment.GetEnvironmentVariable("ACCESS_SECRET");
       var authHeader= context.Request.Headers.Authorization.ToString();

        if (authHeader==null || !authHeader.Contains("Bearer"))
        {
            await _next(context);
            return;
        }

        var token = authHeader.Split(' ')[1];

        var tokenHandler= new JwtSecurityTokenHandler();
        var principal= tokenHandler.ValidateToken(
            token,
            new TokenValidationParameters{
            ValidateIssuer=false,
            ValidateLifetime=true,
            ValidateAudience=false,
            ValidateIssuerSigningKey=true,
            IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret!)),
            ClockSkew=TimeSpan.Zero
            },
            out SecurityToken validatedToken
        );

        context.User=principal;

        await _next(context);
    }
}