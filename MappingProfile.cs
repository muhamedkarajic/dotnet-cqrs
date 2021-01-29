using AutoMapper;
using Webapi.Commands;
using Webapi.Models;
namespace Webapi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StudentDetailsDto, Student>();
            CreateMap<StudentStatusDto, Student>();
            CreateMap<RegisterCommand, Student>();
            CreateMap<EditInfoCommand, Student>();
            CreateMap<EditStatusCommand, Student>();
        }
    }
}