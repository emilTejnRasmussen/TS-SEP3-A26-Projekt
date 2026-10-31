using Context;
using Entities;
using Repository.Exceptions;

namespace Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{

    private readonly AppDbContext context = context;

    public Task<User> AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetSingleByIdAsync(long id)
    {
        var user = await context.Users.FindAsync(id) ?? throw new UserNotFoundException(id);
        
        return user;
    }

    public IQueryable<User> GetMany()
    {
        throw new NotImplementedException();
    }
}