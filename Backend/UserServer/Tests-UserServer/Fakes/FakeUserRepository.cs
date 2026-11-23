using Entities;
using Repository;
using Repository.Exceptions;

namespace Tests_UserServer.Fakes;

/// <summary>
/// In-memory stand-in for <see cref="IUserRepository"/>. It behaves like the real UserRepository:
/// the missing-user lookups throw <see cref="UserNotFoundException"/> and AddAsync gives the user an Id,
/// like the database would.
/// </summary>
public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    private long _nextId = 1;

    public IReadOnlyList<User> Users => _users;
    public int AddCalls { get; private set; }

    // Puts a user into the "database" without counting it as an AddAsync call.
    public User Seed(User user)
    {
        AssignId(user);
        _users.Add(user);
        return user;
    }

    public Task<User> AddAsync(User entity)
    {
        AddCalls++;
        AssignId(entity);
        _users.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(User entity) => Task.CompletedTask;

    public Task DeleteAsync(long id)
    {
        _users.Remove(FindById(id));
        return Task.CompletedTask;
    }

    public Task<User> GetSingleByIdAsync(long id) => Task.FromResult(FindById(id));

    public IQueryable<User> GetMany() => _users.AsQueryable();

    public Task<User> GetByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email)
                   ?? throw new UserNotFoundException(email);

        return Task.FromResult(user);
    }

    public Task<bool> EmailAlreadyExistsAsync(string email) =>
        Task.FromResult(_users.Any(u => u.Email == email));

    private User FindById(long id) =>
        _users.FirstOrDefault(u => u.Id == id) ?? throw new UserNotFoundException(id);

    // User.Id has a private setter (the database generates it), so the fake sets it through reflection.
    private void AssignId(User user) =>
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, _nextId++);
}
