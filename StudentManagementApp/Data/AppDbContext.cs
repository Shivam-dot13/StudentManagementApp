using Microsoft.EntityFrameworkCore;
using StudentManagementApp.Models;

namespace StudentManagementApp.Data
{
    public class AppDbContext : DbContext
    {
        // This tells EF Core that you want a "Students" table based on your "Student" model
        public DbSet<Student> Students { get; set; } = null!;

        // This configures the connection to use a local SQLite file
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=students.db");
        }
    }
}