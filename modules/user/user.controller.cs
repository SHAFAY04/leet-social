

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class UserController: ControllerBase
{

    IUserService _userService;
    
    public UserController(
        IUserService userService
    ){
        _userService=userService;
    }

    [HttpPost("edit")]
    [Roles("User")]
    public IActionResult editProfile(
        [FromForm] UpdateUserProfileDto payload
        )
    {
        Guid userId = Guid.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value
    );
        var response=_userService.editProfile(payload,userId);

        return Ok(response);
    }
}