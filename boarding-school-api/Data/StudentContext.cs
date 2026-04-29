using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Data
{
    public class StudentContext:DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Models.Student>().ToTable("Student");
        }
    }
}
