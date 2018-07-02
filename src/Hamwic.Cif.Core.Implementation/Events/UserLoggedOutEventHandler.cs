using System.Threading.Tasks;
using Hamwic.Cif.Core.Events;
using Hamwic.Core.Events;
using Serilog;

namespace Hamwic.Cif.Core.Implementation.Events
{
    public class UserLoggedOutEventHandler : IDomainEventHandler<UserLoggedOutEvent>
    {
        public void Handle(UserLoggedOutEvent evt)
        {
            Log.Information($"{evt.User.UserName} logged in");
        }

        public Task HandleAsync(UserLoggedOutEvent evt)
        {
            //TODO replace with activity log entry in the database
            Log.Information($"{evt.User.UserName} logged in");
            return Task.CompletedTask;
        }
    }
}