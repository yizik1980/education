using Microsoft.AspNetCore.Mvc;
using boarding_school_api.Models;

namespace boarding_school_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingSchoolsController : ControllerBase
    {
        private static readonly List<BoardingSchool> Schools = new List<BoardingSchool>
        {
            new BoardingSchool { Id = 1, Name = "Oakwood Academy", PupilsCount = 250, AverageAge = 14.5 },
            new BoardingSchool { Id = 2, Name = "Pinecrest International", PupilsCount = 180, AverageAge = 15.2 },
            new BoardingSchool { Id = 3, Name = "Riverdale Prep", PupilsCount = 320, AverageAge = 13.8 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<BoardingSchool>> Get()
        {
            return Ok(Schools);
        }

        [HttpGet("{id}")]
        public ActionResult<BoardingSchool> Get(int id)
        {
            var school = Schools.FirstOrDefault(s => s.Id == id);
            if (school == null)
            {
                return NotFound();
            }
            return Ok(school);
        }

        [HttpPost("critical-incident")]
        public IActionResult TriggerCriticalIncident([FromBody] string message)
        {
            // This endpoint simulates a critical incident that should be logged to the logging service
            throw new Exception($"Critical Incident: {message}");
        }
    }
}
