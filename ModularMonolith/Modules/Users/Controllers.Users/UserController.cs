using Application.Users;
using Application.Users.Commands;
using Application.Users.Queries;
using Controllers.Users.Requests;
using Domain.Users.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Users;

[ApiController]
public class UserController(UserCommands userCommands, UserQueries userQueries) : ControllerBase
{
    [HttpGet(Routes.Users)]
    public async Task<IList<User>> GetUsers()
    {
        return await userQueries.GetUsers();
    }    
    
    [HttpGet(Routes.TheUser)]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await userQueries.Get(id);
        if (user is null) return NotFound();
        return user;
    }    
    
    [HttpPost(Routes.Users)]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] UserPayload payload)
    {
        var id = await userCommands.CreateUser(payload.FullName, payload.Email, payload.UserType);
        return Created($"/{Routes.Users}/{id}", id);
    }    
    
    [HttpPut(Routes.TheUser)]
    public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserPayload payload)
    {
        await userCommands.UpdateUser(id, payload.FullName, payload.Email);
        return NoContent();
    }
}