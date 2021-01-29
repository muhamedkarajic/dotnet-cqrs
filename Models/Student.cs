
namespace Webapi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
    }

    public sealed class StudentDetailsDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public sealed class NewStudentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public sealed class StudentStatusDto
    {
        public bool Status { get; set; }
    }

    
}