using CSharpFunctionalExtensions;

namespace Webapi.Commands
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<TCommand> where TCommand: ICommand
    {
        Result Handle(TCommand command);
    } 
}