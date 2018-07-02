using System.Threading.Tasks;

namespace Hamwic.Core.Commands
{
    public interface ICommandHandler<in TCommand>
    {
        void Handle(TCommand command);
        Task HandleAsync(TCommand command);
    }
}