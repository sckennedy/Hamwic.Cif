namespace Hamwic.Core.Events
{
    public interface IEventDispatcher
    {
        void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent;
        void Raise<TEvent>(TEvent eventToRaise) where TEvent : IDomainEvent;
        System.Threading.Tasks.Task RaiseAsync<TEvent>(TEvent eventToRaise) where TEvent : IDomainEvent;
    }
}