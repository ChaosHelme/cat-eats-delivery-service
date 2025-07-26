using CatEats.UserService.Application.DTOs;
using CatEats.UserService.Infrastructure;

namespace CatEats.UserService.Application.Queries.Handlers;

public class GetAvailableRidersQueryHandler(IUserRepository userRepository) : IQueryHandler<GetAvailableRidersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Query(GetAvailableRidersQuery query, CancellationToken cancellationToken)
    {
        var riders = await userRepository.GetRidersAsync(cancellationToken);
        return riders.Select(UserDto.FromDomain);
    }
}