using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hamwic.Core.Commands;
using Hamwic.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hamwic.Cif.Web.Framework
{
    public class CommandProcessor : ICommandProcessor
    {
        private IServiceProvider Services { get; }

        public CommandProcessor(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
            CommandsProcessed = new List<ICommand>();
        }

        public IList<ICommand> CommandsProcessed { get; }

        public bool CommandWasProcessed<TCommand>() where TCommand : ICommand
        {
            return Enumerable.Any<ICommand>(CommandsProcessed, x => x is TCommand);
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            CommandsProcessed.Add(command);

            Argument.IsNull(command, nameof(command));

            var handler = ServiceProviderServiceExtensions.GetRequiredService<ICommandHandler<TCommand>>(Services);
            if (handler == null)
                throw new CommandHandlerNotFoundException(typeof(TCommand));

#if DEBUG
            Log.Logger.Debug("Dispatching {0} to {1}", command.GetType().Name, handler.GetType().Name);
#endif
            handler.Handle(command);
        }

        public Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            return Task.Run((Action)(() => Execute(command)));
        }
    }
}