using boarding_school_api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Data
{
    public class EducationPlaceSummaryContext : DbContext
    {
        public EducationPlaceSummaryContext(DbContextOptions<EducationPlaceSummaryContext> options)
            : base(options)
        {
        }

        public DbSet<EducationPlaceSummary> EducationPlaceSummaryDataSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EducationPlaceSummary>().ToTable("BoardingSchools");

            modelBuilder.Entity<EducationPlaceSummary>(eb =>
            {
                eb.HasNoKey();
            });
        }

      
    }
}
