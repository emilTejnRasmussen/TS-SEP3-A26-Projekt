using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace UserAPI.Controller;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    
    
    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetSingleAsync([FromRoute] long userId)
    {
        var response = await userService.GetUserByIdAsync(userId);
        
        return Ok(response);
    }
    
}