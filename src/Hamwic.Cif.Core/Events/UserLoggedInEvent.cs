using Hamwic.Cif.Core.Entities;
using Hamwic.Core.Events;

namespace Hamwic.Cif.Core.Events
{
    public class UserLoggedInEvent : IDomainEvent
    {
        public UserLoggedInEvent(ApplicationUser user)
        {
            User = user;
        }

        public ApplicationUser User { get; }

    }
}