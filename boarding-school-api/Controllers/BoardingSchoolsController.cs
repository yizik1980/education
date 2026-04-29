using Azure.Identity;
using boarding_school_api.Data;
using boarding_school_api.Infrastructure;
using boarding_school_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace boarding_school_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BoardingSchoolsController : ControllerBase
    {
        private readonly IBoardingSchoolQuery _context;
        private readonly IEducationPlaceSummaryRepo _summeryContext;

        public BoardingSchoolsController(IBoardingSchoolQuery context, IEducationPlaceSummaryRepo summeryContext)
        {
            _context = context;
            _summeryContext = summeryContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActiveStudentsByPlace>>> Get()
        {
            var schools = await _context.GetAllBoardingSchoolsSPAsync();
            return Ok(schools);
        }


        [HttpPost("summary")]
        public async Task<IActionResult> Post([FromBody] BoardeSchoolRequest req)
        {
            var result = await _summeryContext.GetSummaryAsync(req.City, req.MinStudents);
            return Ok(result);
        }


        [HttpPost("critical-incident")]
        public IActionResult TriggerCriticalIncident([FromBody] string message)
        {
            // This endpoint simulates a critical incident that should be logged to the logging service
            throw new Exception($"Critical Incident: {message}");
        }
    }
}
