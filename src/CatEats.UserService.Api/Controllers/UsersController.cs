using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.Commands.Handlers;
using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Application.Queries;
using CatEats.UserService.Application.Queries.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace CatEats.UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ILogger<UsersController> logger) : ControllerBase
{
    [HttpPost("customers")]
    public async Task<ActionResult<UserDto>> RegisterCustomer([FromBody] RegisterCustomerCommand command,
        [FromServices] ICommandHandler<RegisterCustomerCommand, UserDto> commandHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await commandHandler.Handle(command, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new {id = result.Id}, result);
        } catch (InvalidOperationException ex)
        {
            logger.LogInformation(ex.ToString());
            return BadRequest(new {message = ex.Message});
        } catch (ArgumentException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpPost("riders")]
    public async Task<ActionResult<UserDto>> RegisterRider([FromBody] RegisterRiderCommand command,
        [FromServices] ICommandHandler<RegisterRiderCommand, UserDto> commandHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await commandHandler.Handle(command, cancellationToken);
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
    public async Task<ActionResult<UserDto>> GetUser(Guid id,
        [FromServices] IQueryHandler<GetUserByIdQuery, UserDto?> queryHandler,
        CancellationToken cancellationToken)
    {
        var user = await queryHandler.Query(new  GetUserByIdQuery(id), cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email, 
        [FromServices] IQueryHandler<GetUserByEmailQuery, UserDto?> queryHandler, 
        CancellationToken cancellationToken)
    {
        var user = await queryHandler.Query(new GetUserByEmailQuery(email), cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("riders/available")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAvailableRiders(
        [FromServices] IQueryHandler<GetAvailableRidersQuery, IEnumerable<UserDto>> queryHandler,
        CancellationToken cancellationToken)
    {
        var riders = await queryHandler.Query(new GetAvailableRidersQuery(), cancellationToken);
        return Ok(riders);
    }

    [HttpPost("{id:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid id, 
        [FromBody] (string street, string postalCode, string city, string country, double latitude, double longitude, bool isDefault) address,
        [FromServices] ICommandHandler<AddAddressCommand> commandHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            await commandHandler.Handle(new AddAddressCommand(id, address.city, address.street, address.country,
                address.postalCode, address.latitude, address.longitude, address.isDefault), cancellationToken);

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
    public async Task<IActionResult> UpdateLastLogin(Guid id,
        [FromServices] ICommandHandler<UpdateLastLoginCommand> commandHandler,
        CancellationToken cancellationToken)
    {
        await commandHandler.Handle(new UpdateLastLoginCommand(id), cancellationToken);
        return NoContent();
    }
}