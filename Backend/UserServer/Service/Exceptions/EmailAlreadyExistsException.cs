namespace Exceptions;

public class EmailAlreadyExistsException(string email) 
    : Exception($"Email '{email}' is already registered");