using Domain.Tickets.Primitives;

namespace Domain.Tickets.Entities
{
    public class User : Entity, IAmAnAggregateRoot
    {
        private User(Guid id, Name fullName, Email email) : base(id)
        {
            FullName = fullName;
            Email = email;
        }
        
        public Name FullName { get; private set; }
        public Email Email { get; private set; }
        
        public static User Create(Guid id, Name fullName, Email email)
        {
            return new User(id, fullName, email);
        }
                
        public void UpdateName(Name newFullName)
        {
            FullName = newFullName;
        }
        
        public void UpdateEmail(Email newEmail)
        {
            Email = newEmail;
        }
    }
}
    