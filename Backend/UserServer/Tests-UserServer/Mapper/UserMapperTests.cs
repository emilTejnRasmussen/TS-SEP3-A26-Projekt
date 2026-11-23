using DTO.User;
using Entities;
using Mapper;

namespace Tests_UserServer.Mapper;

public class UserMapperTests
{
    private static User UserWithId(long id, string name, string email, string? location)
    {
        var user = new User(name, email) { Location = location };
        // User.Id has a private setter (the database generates it), so it is set through reflection here.
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);
        return user;
    }

    [Test]
    public void ToDto_CopiesIdNameEmailAndLocation()
    {
        var dto = UserMapper.ToDto(UserWithId(42, "Anna", "anna@example.com", "Horsens"));

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(42));
            Assert.That(dto.Name, Is.EqualTo("Anna"));
            Assert.That(dto.Email, Is.EqualTo("anna@example.com"));
            Assert.That(dto.Location, Is.EqualTo("Horsens"));
        });
    }

    [Test]
    public void ToDto_UserWithoutLocation_GivesNullLocation()
    {
        var dto = UserMapper.ToDto(UserWithId(1, "Anna", "anna@example.com", null));

        Assert.That(dto.Location, Is.Null);
    }

    [Test]
    public void ToDto_DoesNotExposeThePasswordHash()
    {
        // UserResponse is what the API sends back, so it must not carry any password data.
        var properties = typeof(UserResponse).GetProperties().Select(p => p.Name);

        Assert.That(properties, Does.Not.Contain("HashedPassword").And.Not.Contain("Password"));
    }
}
