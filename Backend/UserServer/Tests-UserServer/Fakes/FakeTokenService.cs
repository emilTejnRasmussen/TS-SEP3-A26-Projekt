using Entities;
using Service.Interface;

namespace Tests_UserServer.Fakes;

/// <summary>
/// Stand-in for <see cref="ITokenService"/>. It returns a predictable token and remembers
/// which users a token was requested for, so tests do not need real JWT configuration.
/// </summary>
public class FakeTokenService : ITokenService
{
    private readonly List<User> _requestedFor = [];

    public IReadOnlyList<User> RequestedFor => _requestedFor;
    public int GenerateCalls => _requestedFor.Count;

    public static string TokenFor(User user) => $"token-{user.Id}-{user.Email}";

    public string GenerateToken(User user)
    {
        _requestedFor.Add(user);
        return TokenFor(user);
    }
}
