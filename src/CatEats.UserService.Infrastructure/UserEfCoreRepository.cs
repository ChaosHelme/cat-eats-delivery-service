using AutoMapper;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Domain.Aggregates;
using CatEats.UserService.Domain.Enumerations;
using CatEats.UserService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatEats.UserService.Infrastructure;

public class UserEfCoreRepository(UserDbContext context, IMapper mapper) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var userEntity = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return mapper.Map<User>(userEntity);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userEntity = await context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return mapper.Map<User>(userEntity);
    }

    public async Task<IEnumerable<User>> GetRidersAsync(CancellationToken cancellationToken = default)
    {
        var userEntities = await context.Users
            .Where(u => u.Role == UserRole.Rider && u.Status == UserStatus.Active)
            .ToListAsync(cancellationToken);
        
        return mapper.Map<IEnumerable<User>>(userEntities);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(mapper.Map<UserEntity>(user));
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(mapper.Map<UserEntity>(user));
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}