using System;
using System.Collections.Generic;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Webapi.Commands;
using Webapi.Data;
using Webapi.Models;
using Webapi.Queries;

namespace Webapi.Controllers
{

    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        public ApplicationDbContext context;
        public IMapper mapper;
        public Messages messages;
        public StudentsController(ApplicationDbContext context, IMapper mapper, Messages messages)
        {
            this.context = context;
            this.messages = messages;
            this.mapper = mapper;
        }

        [HttpGet]
        public List<Student> GetAll(bool? status, string email) // query
        {
            return messages.Dispatch(new GetListQuery(status, email));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Register([FromBody] NewStudentDto dto) // command
        {
            var command = new RegisterCommand
            {
                Name = dto.Name,
                Email = dto.Email
            };

            Result result = messages.Dispatch(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPut]
        [Route("{id}/[action]")]
        public IActionResult EditInfo(int id, [FromBody] StudentDetailsDto dto) // command
        {
            var command = new EditInfoCommand
            {
                Id = id,
                Name = dto.Name,
                Email = dto.Email
            };

            Result result = messages.Dispatch(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPut]
        [Route("{id}/[action]")]
        public IActionResult EditStatus(int id, [FromBody] StudentStatusDto dto) // command
        {
            var command = new EditStatusCommand
            {
                Id = id,
                Status = dto.Status
            };

            Result result = messages.Dispatch(command);

            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}
