using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CatEats.UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserApplicationService userService, ILogger<UsersController> logger)
    : ControllerBase
{
    [HttpPost("customers")]
    public async Task<ActionResult<UserDto>> RegisterCustomer([FromBody] RegisterCustomerCommand command)
    {
        try
        {
            var result = await userService.RegisterCustomerAsync(command);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("riders")]
    public async Task<ActionResult<UserDto>> RegisterRider([FromBody] RegisterRiderCommand command)
    {
        try
        {
            var result = await userService.RegisterRiderAsync(command);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("riders/available")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAvailableRiders()
    {
        var riders = await userService.GetAvailableRidersAsync();
        return Ok(riders);
    }

    [HttpPost("{id:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid id, [FromBody] AddAddressCommand command)
    {
        try
        {
            await userService.AddAddressAsync(id, command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/last-login")]
    public async Task<IActionResult> UpdateLastLogin(Guid id)
    {
        await userService.UpdateLastLoginAsync(id);
        return NoContent();
    }
}