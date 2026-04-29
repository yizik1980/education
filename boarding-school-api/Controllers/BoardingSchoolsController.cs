using Microsoft.AspNetCore.Mvc;
using boarding_school_api.Models;
using boarding_school_api.Data;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingSchoolsController : ControllerBase
    {
        private readonly BoardingSchoolContext _context;

        public BoardingSchoolsController(BoardingSchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardingSchool>>> Get()
        {
            var schools = await _context.GetAllBoardingSchoolsSPAsync();
            return Ok(schools);
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<BoardingSchool>> Get(int id)
        //{
        //    var school = await _context.GetBoardingSchoolByIdSPAsync(id);
        //    if (school == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(school);
        //}

        [HttpPost]
        public async Task<ActionResult<BoardingSchool>> Post(BoardingSchool school)
        {
            await _context.InsertBoardingSchoolSPAsync(school);
            return Ok(school);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, BoardingSchool school)
        {
            if (id != school.Id)
            {
                return BadRequest();
            }

            await _context.UpdateBoardingSchoolSPAsync(school);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _context.DeleteBoardingSchoolSPAsync(id);
            return NoContent();
        }

        [HttpPost("critical-incident")]
        public IActionResult TriggerCriticalIncident([FromBody] string message)
        {
            // This endpoint simulates a critical incident that should be logged to the logging service
            throw new Exception($"Critical Incident: {message}");
        }
    }
}
