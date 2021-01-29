using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Webapi.Commands;

namespace Webapi.Decorators
{
	public class AuditLogginDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
	{
		private ICommandHandler<TCommand> _handler;

		public AuditLogginDecorator(ICommandHandler<TCommand> handler)
		{
			_handler = handler;
		}


		public Result Handle(TCommand command)
		{
			string commandJson = JsonConvert.SerializeObject(command);

			Console.WriteLine($"Command of type {command.GetType().Name}: { commandJson }");

			return _handler.Handle(command);
		}
	}
}
