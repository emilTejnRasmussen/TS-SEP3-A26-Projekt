using Entities;

namespace Repository;

public interface IUserRepository
{
    Task<User> AddAsync(User entity);
    Task UpdateAsync(User entity);
    Task DeleteAsync(int id);
    Task<User> GetSingleByIdAsync(long id);
    IQueryable<User> GetMany();
}