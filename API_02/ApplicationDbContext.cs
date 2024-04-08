using API_02.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API_02
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
    {
        public DbSet<Student> Students { get; set; }

    }
}
