namespace Repository.Exceptions;

public class UserNotFoundException(long id) : Exception($"User with id {id} not found");