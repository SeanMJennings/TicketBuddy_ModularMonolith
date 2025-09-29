namespace Controllers.Users;

public static class Routes
{
    public const string Users = "users";
    public const string TheUser = $"{Users}/{{id:guid}}";
}