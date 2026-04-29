using boarding_school_api.Infrastructure;
using boarding_school_api.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class StudentsController(IStudentsRepository studentRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await studentRepository.GetAllStudentsAsync());

    [HttpGet("place/{placeId}")]
    public async Task<IActionResult> GetByPlace(int placeId) =>
        Ok(await studentRepository.GetStudentsByPlaceIdAsync(placeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Student student)
    {
        var created = await studentRepository.InsertNewStudent(student);
        return CreatedAtAction(nameof(GetAll), new { id = created }, created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Student student)
    {
        var updated = await studentRepository.UpdateStudent(student);
        return Ok(updated);
    }

    [HttpDelete("{studentId}")]
    public async Task<IActionResult> Delete(int studentId)
    {
        await studentRepository.DeleteStudent(studentId);
        return NoContent();
    }
}