using boarding_school_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace boarding_school_api.Data
{
    public class BoardingSchoolContext : DbContext
    {
        public BoardingSchoolContext(DbContextOptions<BoardingSchoolContext> options)
            : base(options)
        {
        }

        public DbSet<BoardingSchool> BoardingSchools { get; set; }
        public DbSet<ActiveStudentsByPlace> ActiveStudentsByPlace { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BoardingSchool>().ToTable("BoardingSchools");

            modelBuilder.Entity<ActiveStudentsByPlace>(eb =>
            {
                eb.HasNoKey();
                eb.ToView("vw_ActiveStudentsByPlace");
            });
        }

        // Stored Procedure Methods
        public async Task<IEnumerable<ActiveStudentsByPlace>> GetAllBoardingSchoolsSPAsync()
        {
            return await ActiveStudentsByPlace
                .FromSqlRaw("SELECT EducationPlaceId,PlaceName,City,ActiveStudentCount,AvrageAges FROM vw_ActiveStudentsByPlace")
                .ToListAsync();
        }

        public async Task<BoardingSchool?> GetBoardingSchoolByIdSPAsync(int id)
        {
            var result = await BoardingSchools
                .FromSqlRaw("EXEC GetBoardingSchoolById @Id = {0}", id)
                .ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task InsertBoardingSchoolSPAsync(BoardingSchool school)
        {
            await Database.ExecuteSqlRawAsync(
                "EXEC InsertBoardingSchool @Name = {0}, @PupilsCount = {1}, @AverageAge = {2}",
                school.Name, school.PupilsCount, school.AverageAge);
        }

        public async Task UpdateBoardingSchoolSPAsync(BoardingSchool school)
        {
            await Database.ExecuteSqlRawAsync(
                "EXEC UpdateBoardingSchool @Id = {0}, @Name = {1}, @PupilsCount = {2}, @AverageAge = {3}",
                school.Id, school.Name, school.PupilsCount, school.AverageAge);
        }

        public async Task DeleteBoardingSchoolSPAsync(int id)
        {
            await Database.ExecuteSqlRawAsync("EXEC DeleteBoardingSchool @Id = {0}", id);
        }
    }
}
