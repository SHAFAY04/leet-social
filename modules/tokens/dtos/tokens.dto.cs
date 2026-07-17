public record GenerateTokensResponse(string accessToken, string refreshToken);

public record GenerateAccessTokenResponse(string accessToken);

public record RefreshTheAccessTokenResponse(string accessToken);