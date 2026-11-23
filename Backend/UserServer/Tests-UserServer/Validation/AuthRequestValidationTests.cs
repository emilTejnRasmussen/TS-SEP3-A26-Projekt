using System.ComponentModel.DataAnnotations;
using DTO.Auth;

namespace Tests_UserServer.Validation;

/// <summary>
/// The rules on RegisterRequest and LoginRequest are enforced by the API before the services run
/// (invalid input gives 400), so they are tested here with the same DataAnnotations validator.
/// </summary>
public class AuthRequestValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    private static bool FailsOn(List<ValidationResult> results, string property) =>
        results.Any(r => r.MemberNames.Contains(property));

    private static RegisterRequest ValidRegister() => new()
    {
        Name = "Anna",
        Email = "anna@example.com",
        Password = "hemmeligt-password",
        Location = "Horsens"
    };

    private static LoginRequest ValidLogin() => new()
    {
        Email = "anna@example.com",
        Password = "hemmeligt-password"
    };

    // ---------------------------------------------------------------- register

    [Test]
    public void Register_ValidRequest_HasNoErrors()
    {
        Assert.That(Validate(ValidRegister()), Is.Empty);
    }

    [Test]
    public void Register_LocationIsOptional()
    {
        var request = ValidRegister();
        request.Location = null;

        Assert.That(Validate(request), Is.Empty);
    }

    [TestCase("")]
    [TestCase(null)]
    public void Register_MissingName_Fails(string? name)
    {
        var request = ValidRegister();
        request.Name = name!;

        Assert.That(FailsOn(Validate(request), nameof(RegisterRequest.Name)), Is.True);
    }

    [TestCase("")]
    [TestCase("ikke-en-email")]
    [TestCase("mangler@")]
    public void Register_InvalidEmail_Fails(string email)
    {
        var request = ValidRegister();
        request.Email = email;

        Assert.That(FailsOn(Validate(request), nameof(RegisterRequest.Email)), Is.True);
    }

    [TestCase("")]
    [TestCase("kort")]
    [TestCase("1234567")]
    public void Register_PasswordShorterThanEightCharacters_Fails(string password)
    {
        var request = ValidRegister();
        request.Password = password;

        Assert.That(FailsOn(Validate(request), nameof(RegisterRequest.Password)), Is.True);
    }

    [Test]
    public void Register_PasswordOfExactlyEightCharacters_IsAccepted()
    {
        var request = ValidRegister();
        request.Password = "12345678";

        Assert.That(Validate(request), Is.Empty);
    }

    [Test]
    public void Register_EmptyRequest_ReportsNameEmailAndPassword()
    {
        var results = Validate(new RegisterRequest());

        Assert.Multiple(() =>
        {
            Assert.That(FailsOn(results, nameof(RegisterRequest.Name)), Is.True);
            Assert.That(FailsOn(results, nameof(RegisterRequest.Email)), Is.True);
            Assert.That(FailsOn(results, nameof(RegisterRequest.Password)), Is.True);
        });
    }

    // ---------------------------------------------------------------- login

    [Test]
    public void Login_ValidRequest_HasNoErrors()
    {
        Assert.That(Validate(ValidLogin()), Is.Empty);
    }

    [TestCase("")]
    [TestCase("ikke-en-email")]
    public void Login_InvalidEmail_Fails(string email)
    {
        var request = ValidLogin();
        request.Email = email;

        Assert.That(FailsOn(Validate(request), nameof(LoginRequest.Email)), Is.True);
    }

    [Test]
    public void Login_MissingPassword_Fails()
    {
        var request = ValidLogin();
        request.Password = "";

        Assert.That(FailsOn(Validate(request), nameof(LoginRequest.Password)), Is.True);
    }

    [Test]
    public void Login_ShortPassword_IsNotRejectedByValidation()
    {
        // Login only requires a password to be present; the length rule belongs to register.
        var request = ValidLogin();
        request.Password = "kort";

        Assert.That(Validate(request), Is.Empty);
    }
}
