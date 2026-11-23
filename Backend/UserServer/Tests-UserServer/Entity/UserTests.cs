using Entities;

namespace Tests_UserServer.Entity;

/// <summary>Creating a <see cref="User"/> - the entity that register stores.</summary>
public class UserTests
{
    [Test]
    public void Constructor_SetsNameAndEmail()
    {
        var user = new User("Anna", "anna@example.com");

        Assert.Multiple(() =>
        {
            Assert.That(user.Name, Is.EqualTo("Anna"));
            Assert.That(user.Email, Is.EqualTo("anna@example.com"));
        });
    }

    [Test]
    public void NewUser_HasNoLocationAndNoIdYet()
    {
        var user = new User("Anna", "anna@example.com");

        Assert.Multiple(() =>
        {
            Assert.That(user.Location, Is.Null);
            Assert.That(user.Id, Is.Zero, "the database generates the id when the user is saved");
        });
    }

    [Test]
    public void Properties_CanBeUpdated()
    {
        var user = new User("Anna", "anna@example.com")
        {
            Location = "Horsens",
            HashedPassword = "hash"
        };

        user.Name = "Anne";
        user.Email = "anne@example.com";

        Assert.Multiple(() =>
        {
            Assert.That(user.Name, Is.EqualTo("Anne"));
            Assert.That(user.Email, Is.EqualTo("anne@example.com"));
            Assert.That(user.Location, Is.EqualTo("Horsens"));
            Assert.That(user.HashedPassword, Is.EqualTo("hash"));
        });
    }
}
