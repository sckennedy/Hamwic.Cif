using Hamwic.Cif.Core.Entities;
using Hamwic.Core.Events;

namespace Hamwic.Cif.Core.Events
{
    public class UserLoggedOutEvent : IDomainEvent
    {
        public UserLoggedOutEvent(ApplicationUser user)
        {
            User = user;
        }

        public ApplicationUser User { get; }

    }
}