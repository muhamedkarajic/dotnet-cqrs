using Microsoft.EntityFrameworkCore;
using Webapi.Models;

namespace Webapi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Student> Students { get; set; }

        
    }
}