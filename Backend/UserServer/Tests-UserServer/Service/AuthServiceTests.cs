using DTO.Auth;
using Tests.UnitTests.Fakes; 

namespace Tests_UserServer.Service;

[TestFixture]
public class AuthServiceTests
{
    private const string TakenEmail = "TakenEmail";
    private const string Token = "jwt-token";

    private FakeUserRepository users = null!;
}

