using AutoMapper;
using Webapi.Data;
using Webapi.Models;
using CSharpFunctionalExtensions;
using System;
using Webapi.Attributes;

namespace Webapi.Commands
{
    public sealed class EditInfoCommand : ICommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [AuditLog]
        [DatabaseRetry]
        internal sealed class EditInfoCommandHandler : ICommandHandler<EditInfoCommand>
        {
            IMapper mapper;
            ApplicationDbContext context;
            public EditInfoCommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                this.mapper = mapper;
                this.context = context;
            }

            public Result Handle(EditInfoCommand command)
            {
                try
                {
                    Student student = context.Students.Find(command.Id);

                    if (student == null)
                    {
                        throw new Exception("Student not found.");
                    }

                    mapper.Map(command, student);
                    // student.Email = dto.Email;
                    // student.Name = dto.Name;

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
}