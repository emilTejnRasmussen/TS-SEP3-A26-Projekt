using DTO.Auth;
using Entities;
using Exceptions;
using Microsoft.AspNetCore.Identity;
using Repository.Exceptions;
using Service;
using Services.Exceptions;
using Tests_UserServer.Fakes;

namespace Tests_UserServer.Service;

/// <summary>
/// Unit tests for <see cref="AuthService"/>: register (which creates the user) and login.
/// The repository and token service are fakes; the password hasher is the real ASP.NET one,
/// so the tests also prove that passwords are hashed and verified for real.
/// </summary>
public class AuthServiceTests
{
    private const string Email = "anna@example.com";
    private const string Password = "hemmeligt-password";

    private FakeUserRepository repo = null!;
    private FakeTokenService tokens = null!;
    private PasswordHasher<User> hasher = null!;
    private AuthService authService = null!;

    [SetUp]
    public void SetUp()
    {
        repo = new FakeUserRepository();
        tokens = new FakeTokenService();
        hasher = new PasswordHasher<User>();
        authService = new AuthService(repo, hasher, tokens);
    }

    private static RegisterRequest RegisterRequestFor(
        string name = "Anna",
        string email = Email,
        string password = Password,
        string? location = "Horsens") => new()
    {
        Name = name,
        Email = email,
        Password = password,
        Location = location
    };

    private static LoginRequest LoginRequestFor(string email = Email, string password = Password) => new()
    {
        Email = email,
        Password = password
    };

    // A user that already exists in the "database", with a correctly hashed password.
    private User SeedUser(string email = Email, string password = Password)
    {
        var user = new User("Anna", email);
        user.HashedPassword = hasher.HashPassword(user, password);
        return repo.Seed(user);
    }

    // register / create user

    [Test]
    public async Task Register_ValidRequest_ReturnsTokenFromTokenService()
    {
        var response = await authService.RegisterAsync(RegisterRequestFor());

        var created = repo.Users.Single();
        Assert.That(response.Token, Is.EqualTo(FakeTokenService.TokenFor(created)));
    }

    [Test]
    public async Task Register_ValidRequest_CreatesExactlyOneUser()
    {
        await authService.RegisterAsync(RegisterRequestFor());

        Assert.That(repo.AddCalls, Is.EqualTo(1));
        Assert.That(repo.Users, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Register_ValidRequest_StoresNameEmailAndLocation()
    {
        await authService.RegisterAsync(RegisterRequestFor(name: "Anna", email: Email, location: "Horsens"));

        var created = repo.Users.Single();
        Assert.Multiple(() =>
        {
            Assert.That(created.Name, Is.EqualTo("Anna"));
            Assert.That(created.Email, Is.EqualTo(Email));
            Assert.That(created.Location, Is.EqualTo("Horsens"));
        });
    }

    [Test]
    public async Task Register_WithoutLocation_StoresNullLocation()
    {
        await authService.RegisterAsync(RegisterRequestFor(location: null));

        Assert.That(repo.Users.Single().Location, Is.Null);
    }

    [Test]
    public async Task Register_ValidRequest_StoresHashedPasswordAndNotThePlainText()
    {
        await authService.RegisterAsync(RegisterRequestFor(password: Password));

        var created = repo.Users.Single();
        Assert.Multiple(() =>
        {
            Assert.That(created.HashedPassword, Is.Not.Null.And.Not.Empty);
            Assert.That(created.HashedPassword, Is.Not.EqualTo(Password));
            Assert.That(hasher.VerifyHashedPassword(created, created.HashedPassword, Password),
                Is.Not.EqualTo(PasswordVerificationResult.Failed));
        });
    }

    [Test]
    public async Task Register_ValidRequest_GeneratesTokenForTheCreatedUser()
    {
        await authService.RegisterAsync(RegisterRequestFor());

        var created = repo.Users.Single();
        Assert.That(tokens.GenerateCalls, Is.EqualTo(1));
        Assert.That(tokens.RequestedFor.Single(), Is.SameAs(created));
        Assert.That(created.Id, Is.GreaterThan(0), "the token is made after the user got its database id");
    }

    [Test]
    public async Task Register_TwoDifferentEmails_CreatesTwoUsers()
    {
        await authService.RegisterAsync(RegisterRequestFor(email: "anna@example.com"));
        await authService.RegisterAsync(RegisterRequestFor(name: "Bo", email: "bo@example.com"));

        Assert.That(repo.Users.Select(u => u.Email), Is.EquivalentTo(new[] { "anna@example.com", "bo@example.com" }));
    }

    [Test]
    public void Register_EmailAlreadyRegistered_ThrowsEmailAlreadyExistsException()
    {
        SeedUser(Email);

        var ex = Assert.ThrowsAsync<EmailAlreadyExistsException>(() => authService.RegisterAsync(RegisterRequestFor(email: Email)));

        Assert.That(ex!.Message, Does.Contain(Email));
    }

    [Test]
    public void Register_EmailAlreadyRegistered_CreatesNoUserAndNoToken()
    {
        SeedUser(Email);

        Assert.ThrowsAsync<EmailAlreadyExistsException>(() => authService.RegisterAsync(RegisterRequestFor(email: Email)));

        Assert.Multiple(() =>
        {
            Assert.That(repo.AddCalls, Is.Zero);
            Assert.That(repo.Users, Has.Count.EqualTo(1));
            Assert.That(tokens.GenerateCalls, Is.Zero);
        });
    }

    [Test]
    public async Task RegisteredUser_CanLogInWithTheSameCredentials()
    {
        await authService.RegisterAsync(RegisterRequestFor());

        var response = await authService.LoginAsync(LoginRequestFor());

        Assert.That(response.Token, Is.EqualTo(FakeTokenService.TokenFor(repo.Users.Single())));
    }

    // ---------------------------------------------------------------- login

    [Test]
    public async Task Login_CorrectPassword_ReturnsTokenForThatUser()
    {
        var user = SeedUser();

        var response = await authService.LoginAsync(LoginRequestFor());

        Assert.That(response.Token, Is.EqualTo(FakeTokenService.TokenFor(user)));
        Assert.That(tokens.RequestedFor.Single(), Is.SameAs(user));
    }

    [Test]
    public async Task Login_CorrectPassword_DoesNotCreateOrChangeUsers()
    {
        SeedUser();

        await authService.LoginAsync(LoginRequestFor());

        Assert.That(repo.AddCalls, Is.Zero);
        Assert.That(repo.Users, Has.Count.EqualTo(1));
    }

    [Test]
    public void Login_WrongPassword_ThrowsInvalidCredentialsException()
    {
        SeedUser(password: Password);

        Assert.ThrowsAsync<InvalidCredentialsException>(() => authService.LoginAsync(LoginRequestFor(password: "forkert-password")));
    }

    [Test]
    public void Login_WrongPassword_GeneratesNoToken()
    {
        SeedUser(password: Password);

        Assert.ThrowsAsync<InvalidCredentialsException>(() => authService.LoginAsync(LoginRequestFor(password: "forkert-password")));

        Assert.That(tokens.GenerateCalls, Is.Zero);
    }

    [Test]
    public void Login_PasswordWithDifferentCasing_IsRejected()
    {
        SeedUser(password: "Hemmeligt-Password");

        Assert.ThrowsAsync<InvalidCredentialsException>(() => authService.LoginAsync(LoginRequestFor(password: "hemmeligt-password")));
    }

    [Test]
    public void Login_UnknownEmail_ThrowsUserNotFoundException()
    {
        // Documents how the code behaves today: the repository throws UserNotFoundException for an unknown email,
        // so the "?? throw new InvalidCredentialsException()" in AuthService.LoginAsync is never reached.
        // The API therefore answers 404 here and not 401 (which also says that the email does not exist).
        var ex = Assert.ThrowsAsync<UserNotFoundException>(() => authService.LoginAsync(LoginRequestFor(email: "findes-ikke@example.com")));

        Assert.That(ex!.Message, Does.Contain("findes-ikke@example.com"));
    }

    [Test]
    public void Login_UnknownEmail_GeneratesNoToken()
    {
        Assert.ThrowsAsync<UserNotFoundException>(() => authService.LoginAsync(LoginRequestFor(email: "findes-ikke@example.com")));

        Assert.That(tokens.GenerateCalls, Is.Zero);
    }
}
