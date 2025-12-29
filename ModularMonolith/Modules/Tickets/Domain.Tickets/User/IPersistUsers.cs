namespace Domain.Tickets.User;

public interface IPersistUsers
{
    public Task Upsert(User user);
}