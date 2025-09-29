using Domain.Users.Primitives;

namespace Controllers.Users.Requests;

public record UserPayload(FullName FullName, Email Email, UserType UserType);
public record UpdateUserPayload(FullName FullName, Email Email);