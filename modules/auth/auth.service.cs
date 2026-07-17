

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1;

public class AuthService : IAuthService
{
    AppDbContext _dbContext;
    ITokensService _tokensService;
    IMediaService _mediaService;
    public AuthService(AppDbContext dbContext, ITokensService tokensService, IMediaService mediaService)
    {
        _dbContext = dbContext;
        _tokensService = tokensService;
        _mediaService = mediaService;
    }
    public async Task<UserAuthResponseDto> Login(UserAuthRequest request)
    {

        var userAuth = await _dbContext.Auth
        .Include(a => a.user)
        .ThenInclude(u => u.profileMedia)
        .Where(a => a.email == request.email)
        .FirstOrDefaultAsync();

        if (userAuth == null)
        {
            throw new UnauthorizedException("Invalid Credentials!");
        }

        bool isValidPass = BCrypt.Net.BCrypt.Verify(request.password, userAuth.hashedPassword);

        if (!isValidPass)
        {
            throw new UnauthorizedException("Invalid Credentials!");
        }

        var tokens = _tokensService.GenerateTokens(userAuth.userId, userAuth.user.username, userAuth.email, userAuth.role);

        userAuth.refreshToken = tokens.refreshToken;
        userAuth.refreshExpiry = DateTime.UtcNow.AddDays(1);

        await _dbContext.SaveChangesAsync();

        string? url = null;
        if (userAuth.user?.profileMedia?.filePath != null)
        {
            url = _mediaService.GetMediaUrl(userAuth.user.profileMedia.filePath);
        }

        UserProfile profile = new UserProfile
        {
            username = userAuth.user.username,
            email = userAuth.email,
            bio = userAuth.user.bio,
            pronouns = userAuth.user.pronouns,
            avatarUrl = url

        };

        return new UserAuthResponseDto(accessToken: tokens.accessToken, refreshToken: tokens.refreshToken, profile);
    }
    public async Task Register(UserAuthRequest request)
    {

        var existing = await _dbContext.Auth.FirstOrDefaultAsync(a => a.email == request.email);

        if (existing != null)
        {
            throw new ConflictException("user already exists");
        }

        var username = request.email.Split('@')[0];
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

        UserEntity newUser = new() { username = username };
        AuthEntity auth = new() { username = username, email = request.email, hashedPassword = hashedPassword, role = RolesEnum.User, user = newUser };
        var newUserAuth = _dbContext.Auth.Add(auth);

        await _dbContext.SaveChangesAsync();

    }
    public async Task<RefreshAuthResponseDto> Refresh(string refreshToken)
    {
        AuthEntity? userAuthRecord;

        if (refreshToken == null)
        {
            throw new UnauthorizedException("no token provided");
        }


        //look we dont need a try catch every level up till the controller because the execution stops and the exception just bubbles up 
        // we are just using an exception here because we have access to the auth table in this service and we want to clear the refreshToken from our authService.
        try
        {
            //validating token and creating new token first because checking database is expensive operation. if token is invalid we dont hit the db.
            var response = _tokensService.RefreshTheAccessToken(refreshToken);

            //if we reach this point we have a new access token so now we can verify if the user is actually logged in.
            userAuthRecord = await _dbContext.Auth.FirstOrDefaultAsync((a) => a.refreshToken == refreshToken);

            if (userAuthRecord == null)
            {
                throw new UnauthorizedException("Unauthorized");
            }

            return new RefreshAuthResponseDto(response.accessToken);

        }
        catch (SecurityTokenException)
        {
            //since we validate the token first and if the token validation fails. there is still a chance that we have a refreshtoken of a user in the database which was
            //initially valid but now might be invalid because of the time expiry. so we can just match the strings and clear the database if token is invalid.

            //have to fetch a record here as well because the catch block will execute as the validation fails
            userAuthRecord = await _dbContext.Auth.FirstOrDefaultAsync((a) => a.refreshToken == refreshToken);

            if (userAuthRecord != null)
            {
                userAuthRecord.refreshToken = null;
                userAuthRecord.refreshExpiry = null;

                await _dbContext.SaveChangesAsync();
            }
            throw new UnauthorizedException("Invalid refresh token");
        }
    }
    public async Task Logout(string refreshToken)
    {

        if (refreshToken == null)
        {
            return;
        }

        var userAuthRecord = await _dbContext.Auth.FirstOrDefaultAsync(a => a.refreshToken == refreshToken);

        if (userAuthRecord == null)
        {
            return;
        }

        userAuthRecord.refreshToken = null;
        userAuthRecord.refreshExpiry = null;

        await _dbContext.SaveChangesAsync();

    }
}