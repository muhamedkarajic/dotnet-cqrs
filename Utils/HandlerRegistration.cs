using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Webapi.Attributes;
using Webapi.Commands;
using Webapi.Decorators;
using Webapi.Queries;

namespace Webapi.Utils
{
	public static class HandlerRegistration
	{
		// 1. Scan the assembly with the domain model for all handlers (command, query)
		public static void AddHandlers(this IServiceCollection services)
		{
			List<Type> handlerTypes = typeof(ICommand).Assembly.GetTypes()
				.Where(x => x.GetInterfaces().Any(IsHandlerInterface))
				.Where(x => x.Name.EndsWith("Handler"))
				.ToList();
			foreach (var type in handlerTypes)
			{
				AddHandler(services, type);
			}
		}

		// 2. Check if the service if of type handler (ICommandHandler || IQueryHandler)
		private static bool IsHandlerInterface(Type type)
		{
			if (!type.IsGenericType)
				return false;

			Type typeDefinition = type.GetGenericTypeDefinition();

			return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(IQueryHandler<,>);
		}

		// 3. gets all Attributes and converts them to Decorators
		private static void AddHandler(IServiceCollection services, Type type)
		{
			object[] attributes = type.GetCustomAttributes(false);

			List<Type> pipeline = attributes
				.Select(ToDecorator)
				.Concat(new[] {type})
				.Reverse()
				.ToList();

			Type interfaceType = type.GetInterfaces().Single(IsHandlerInterface);
			Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);

			services.AddTransient(interfaceType, factory); //register them
		}

		// 4. Finds the right Decorator based on the Attribute
		private static Type ToDecorator(object attribute)
		{
			Type type = attribute.GetType();

			if (type == typeof(DatabaseRetryAttribute))
				return typeof(DatabaseRetryDecorator<>);

			if (type == typeof(AuditLogAttribute))
				return typeof(AuditLogginDecorator<>);

			// other attributes need to be added

			throw new ArgumentException(attribute.ToString());
		}

		// 5. Builds the factory method of all decorators and handlers we found
		private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
		{
			// Gets constructors of each pipeline type
			List<ConstructorInfo> ctors = pipeline.Select(x =>
			{
				Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
				return type.GetConstructors().Single();
			}).ToList();

			// Creates delegates
			Func<IServiceProvider, object> func = provider =>
			{
				object current = null;

				// Go tough all constructors get parametars they require
				foreach (var ctor in ctors)
				{
					List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

					// Resolve them
					object[] parameters = GetParameters(parameterInfos, current, provider);

					// Creates a instance of the type in pipeline
					current = ctor.Invoke(parameters);
				}

				return current;
			};

			return func;
		}

		// 6. Resolves parameters each constructor takes 
		private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
		{
			var result = new object[parameterInfos.Count];

			for (int i = 0; i < parameterInfos.Count; i++)
			{
				result[i] = GetParameter(parameterInfos[i], current, provider);
			}

			return result;
		}

		// 7. If parameter is a handler interface returns the object we created before, else uses dependency injection container to resolve it
		private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
		{
			Type parameterType = parameterInfo.ParameterType;

			if (IsHandlerInterface(parameterType))
				return current;

			object service = provider.GetService(parameterType);

			if (service != null)
				return service;

			throw new ArgumentException($"Type {parameterType} not found");
		}




	}
}
