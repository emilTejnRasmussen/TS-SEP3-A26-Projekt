using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Exceptions;

namespace Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> AddAsync(User entity)
    {
        context.Users.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task UpdateAsync(User entity)
    {
        context.Users.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var user = await GetSingleByIdAsync(id);

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetSingleByIdAsync(long id)
    {
        return await context.Users.FindAsync(id)
               ?? throw new UserNotFoundException(id);
    }

    public IQueryable<User> GetMany()
    {
        return context.Users.AsNoTracking();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await context.Users
                   .FirstOrDefaultAsync(u => u.Email == email)
               ?? throw new UserNotFoundException(email);
    }

    public async Task<bool> EmailAlreadyExistsAsync(string email)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email);
    }
}