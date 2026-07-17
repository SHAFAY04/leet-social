

using WebApplication1;

public interface IAuthService
{
    Task<UserAuthResponseDto> Login(UserAuthRequest request);

    Task Register(UserAuthRequest request);

    Task<RefreshAuthResponseDto> Refresh(string refreshToken);

    Task Logout(string refreshtoken);
    
}