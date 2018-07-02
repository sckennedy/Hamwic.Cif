using System.Threading.Tasks;

namespace Hamwic.Core.Commands
{
    public interface ICommandProcessor
    {
        void Execute<TCommand>(TCommand command) where TCommand : ICommand;
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}