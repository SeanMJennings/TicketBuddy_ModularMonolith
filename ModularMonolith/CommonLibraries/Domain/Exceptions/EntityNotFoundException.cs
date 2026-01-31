namespace Domain.Exceptions;

public class EntityNotFoundException(string entityName, object key)
    : Exception($"{entityName} with id {key} was not found.");