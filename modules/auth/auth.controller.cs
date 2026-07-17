

using Microsoft.AspNetCore.Mvc;
using WebApplication1;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserAuthRequest payload)
    {
        var response = await _authService.Login(payload);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1)
        };

        Response.Cookies.Append("refreshToken", response.refreshToken, cookieOptions);

        return Ok(new { accessToken = response.accessToken });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserAuthRequest payload)
    {
        await _authService.Register(payload);

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];


        //look we dont need a try catch every level up till the controller because the execution stops and the exception just bubbles up 
        // we are just using an exception here because we dont want to handle any http logic like clearing cookies in our service.
        try
        {
            var response = await _authService.Refresh(refreshToken);

            return Ok(response);
        }
        catch (UnauthorizedException)
        {

            Response.Cookies.Delete("refreshToken");
            throw;
        }


    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        await _authService.Logout(refreshToken);

        Response.Cookies.Delete("refreshToken");
        return Ok();
    }
}