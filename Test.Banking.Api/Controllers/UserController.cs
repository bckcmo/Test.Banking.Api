using Microsoft.AspNetCore.Mvc;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateUser(User user)
    {
        var result = await this.userService.Create(user);
        return result.Result is null ?
            this.UnprocessableEntity(result.Errors) :
            this.Created($"/user/{user.Id}", result.Result);
    }
    
    [HttpGet]
    [Route("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ReadUser(int userId)
    {
        var result = await this.userService.Read(userId);
        return result.Result is null ? this.NotFound() : this.Ok(result.Result);
    }
}