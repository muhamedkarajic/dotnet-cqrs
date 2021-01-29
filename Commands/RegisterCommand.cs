using AutoMapper;
using Webapi.Data;
using Webapi.Models;
using CSharpFunctionalExtensions;
using System;
using Webapi.Attributes;

namespace Webapi.Commands
{
    public sealed class RegisterCommand: ICommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    
    [DatabaseRetry]
    [AuditLog]
    public sealed class RegisterCommandHandler: ICommandHandler<RegisterCommand>
    {
        IMapper mapper;
        ApplicationDbContext context;
        public RegisterCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public Result Handle(RegisterCommand command)
        {
            try
            {
                Student student = new Student();
                
                mapper.Map(command, student);

                context.Students.Add(student);
                context.SaveChanges();
            }
            catch (Exception exception)
            {
				throw;
			}

            return Result.Success();
        }
    }


}