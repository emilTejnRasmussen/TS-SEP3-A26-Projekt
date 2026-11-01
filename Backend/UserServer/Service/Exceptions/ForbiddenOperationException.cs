namespace Services.Exceptions;

public class ForbiddenOperationException(string message) : Exception(message);