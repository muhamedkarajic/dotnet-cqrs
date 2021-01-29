using System;
using CSharpFunctionalExtensions;
using Webapi.Commands;
using Webapi.Utils;

namespace Webapi.Decorators
{
    public class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly Config _config;
        
        public DatabaseRetryDecorator(ICommandHandler<TCommand> handler, Config config)
        {
            _handler = handler;
            _config = config;
        }

        public Result Handle(TCommand command)
		{
            for (int i = 0; i <= _config.NumberOfDatabaseRetries; i++)
            {
                try
                {
                    Result result = _handler.Handle(command);
                    return result;
                }
                catch (Exception ex)
				{
					if (i >= _config.NumberOfDatabaseRetries - 1 || !IsDatabaseException(ex))
                    {
                        throw;
                    }
                }
            }
            throw new InvalidOperationException("Should not ver get here.");
        }

        private bool IsDatabaseException(Exception exception)
        {
            string message = exception.Message;

            if(message == null)
                return false;

            return message.Contains("The connection is broken and recovery is not possible")  || message.Contains("The connection is broken and recovery is not possible") || message.Contains("Could not open a connection to SQL Server");
        }
    }
}