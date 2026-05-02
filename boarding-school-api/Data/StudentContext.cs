using boarding_school_api.Models;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Data
{
    public class StudentContext:DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
        }
   
        public DbSet<Student> Students { get; set; }
        public DbSet<EducationPlaceSummary> EducationPlaces { get; set; }  
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .ToTable("Student")
                .HasKey(s => s.StudentId);

            modelBuilder.Entity<EducationPlaceSummary>()
                .ToTable("EducationPlace")
                .HasKey(e => e.EducationPlaceId);
        }
    }
}
