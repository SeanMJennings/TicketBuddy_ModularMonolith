using Application.Users;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Users;

[ApiController]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet(Routes.Users)]
    public async Task<IList<User>> GetUsers()
    {
        return await userService.GetUsers();
    }    
    
    [HttpGet(Routes.TheUser)]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await userService.Get(id);
        if (user is null) return NotFound();
        return user;
    }    
    
    [HttpPost(Routes.Users)]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] UserPayload payload)
    {
        var id = await userService.CreateUser(payload.FullName, payload.Email, payload.UserType);
        return Created($"/{Routes.Users}/{id}", id);
    }    
    
    [HttpPut(Routes.TheUser)]
    public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserPayload payload)
    {
        await userService.UpdateUser(id, payload.FullName, payload.Email);
        return NoContent();
    }
}