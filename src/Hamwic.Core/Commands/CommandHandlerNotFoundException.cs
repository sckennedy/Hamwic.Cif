using System;

namespace Hamwic.Core.Commands
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(Type commandType)
        { }
    }
}