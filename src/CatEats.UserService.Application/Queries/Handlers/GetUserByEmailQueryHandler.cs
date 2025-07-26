using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Infrastructure;

namespace CatEats.UserService.Application.Queries.Handlers;

public class GetUserByEmailQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserByEmailQuery, UserDto?>
{
    public async Task<UserDto?> Query(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);
        return user != null ? UserDto.FromDomain(user) : null;
    }
}