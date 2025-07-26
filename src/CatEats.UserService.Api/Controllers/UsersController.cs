using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CatEats.UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserApplicationService userService, ILogger<UsersController> logger, IMediator mediator)
    : ControllerBase
{
    [HttpPost("customers")]
    public async Task<ActionResult<UserDto>> RegisterCustomer([FromBody] RegisterCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogInformation(ex.ToString());
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("riders")]
    public async Task<ActionResult<UserDto>> RegisterRider([FromBody] RegisterRiderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
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
    public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
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
    public async Task<IActionResult> AddAddress(Guid id, [FromBody] (string street, string postalCode, string city, string country, double latitude, double longitude, bool isDefault) address)
    {
        try
        {
            await mediator.Send(new AddAddressCommand(id, address.city, address.street, address.country,
                address.postalCode, address.latitude, address.longitude, address.isDefault));

            return NoContent();
        } catch (InvalidOperationException ex)
        {
            return BadRequest(new {message = ex.Message});
        } catch (ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpPut("{id:guid}/last-login")]
    public async Task<IActionResult> UpdateLastLogin(Guid id)
    {
        await userService.UpdateLastLoginAsync(id);
        return NoContent();
    }
}