using AutoMapper;
using Webapi.Data;
using Webapi.Models;
using CSharpFunctionalExtensions;
using System;
using Webapi.Attributes;

namespace Webapi.Commands
{
    public sealed class EditStatusCommand : ICommand
    {
        public int Id { get; set; }
        public bool Status { get; set; }
    }
    [DatabaseRetry]
    [AuditLog]
    public sealed class EditStatusCommandHandler : ICommandHandler<EditStatusCommand>
    {
        IMapper mapper;
        ApplicationDbContext context;
        public EditStatusCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public Result Handle(EditStatusCommand command)
        {
            try
            {
                Student student = context.Students.Find(command.Id);
                if (student == null)
                {
                    throw new Exception("Student not found.");
                }

                mapper.Map(command, student);

                context.Students.Update(student);
                context.SaveChanges();
            }
            catch (Exception exception)
            {

                return Result.Failure(exception.Message);
            }

            return Result.Success();
        }
    }


}