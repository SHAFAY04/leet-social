
public interface ITokensService
{
    GenerateTokensResponse GenerateTokens(Guid userId, string username,string email,RolesEnum role);

    GenerateAccessTokenResponse GenerateAccessToken(string userId, string username, string email, string role);
    RefreshTheAccessTokenResponse RefreshTheAccessToken(string refreshToken);
}