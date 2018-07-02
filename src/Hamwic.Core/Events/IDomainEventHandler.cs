using System.Threading.Tasks;

namespace Hamwic.Core.Events
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent evt);
        Task HandleAsync(TEvent evt);
    }
}