using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace UserAPI.Controller;

public class UsersController(IUserService userService) : ApiControllerBase
{
    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetSingleAsync([FromRoute] long userId)
    {
        var response = await userService.GetUserByIdAsync(userId);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetManyAsync([FromQuery] string? nameContains = null)
    {
        var response = await userService.GetManyAsync(nameContains);

        return Ok(response);
    }
}