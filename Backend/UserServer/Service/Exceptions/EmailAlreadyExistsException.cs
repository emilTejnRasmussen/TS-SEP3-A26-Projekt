namespace Exceptions;

public class UsernameAlreadyExistsException(string username) 
    : Exception($"Username '{username}' is already taken");