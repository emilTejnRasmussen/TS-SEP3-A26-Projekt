namespace Repository.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(long id)
        : base($"User with id {id} not found")
    {
    }

    public UserNotFoundException(string email)
        : base($"User with email '{email}' not found")
    {
    }
}