using System.Threading.Tasks;
using Hamwic.Core.Entities;
using Hamwic.Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Hamwic.Cif.Web.Framework
{
    public class DomainEventDispatcher : IEventDispatcher
    {
        private readonly IHttpContextAccessor _context;

        public DomainEventDispatcher(IHttpContextAccessor context)
        {
            _context = context;
        }

        public void Raise<TEvent>(TEvent eventToRaise) where TEvent : IDomainEvent
        {
            Dispatch(eventToRaise);
        }

        public Task RaiseAsync<TEvent>(TEvent eventToRaise) where TEvent : IDomainEvent
        {
            return Task.Run(() => Raise(eventToRaise));
        }

        public void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent
        {
            Argument.IsNull(eventToDispatch, nameof(eventToDispatch));
            foreach (var handler in _context.HttpContext.RequestServices.GetServices<IDomainEventHandler<TEvent>>())
            {
                if (Log.IsEnabled(LogEventLevel.Debug))
                    Log.Debug("Dispatching {0} to {1}", eventToDispatch.GetType().Name, handler.GetType().Name);

                handler.Handle(eventToDispatch);
            }
        }
    }
}