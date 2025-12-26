using Domain.Entities;
using Domain.Tickets.ValueObjects;

namespace Domain.Tickets.Entities
{
    public class User : Entity, IAmAnAggregateRoot
    {
        public User(Guid id, Name fullName, Email email) : base(id)
        {
            FullName = fullName;
            Email = email;
        }
        
        public Name FullName { get; private set; }
        public Email Email { get; private set; }
                
        public void UpdateName(Name newFullName) => FullName = newFullName;
        
        public void UpdateEmail(Email newEmail) => Email = newEmail;
    }
}
    