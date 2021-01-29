using AutoMapper;
using Webapi.Data;
using Webapi.Models;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webapi.Queries
{
    public sealed class GetListQuery: IQuery<List<Student>>
    {
        public bool? Status { get; }
        public string Email { get; }

        public GetListQuery(bool? status, string email)
        {
            Email = email;
            Status = status;
        }
    }
    
    public sealed class GetListQueryHandler: IQueryHandler<GetListQuery, List<Student>>
    {
        IMapper mapper;
        ApplicationDbContext context;
        public GetListQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public List<Student> Handle(GetListQuery command)
        {
            return context.Students
                .Where(s => s.Status == command.Status || command.Status == null)
                .Where(s => s.Email == command.Email || String.IsNullOrEmpty(command.Email))
                .ToList();
        }
    }


}