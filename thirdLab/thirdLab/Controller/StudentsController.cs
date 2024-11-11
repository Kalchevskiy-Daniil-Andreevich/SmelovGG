using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using thirdLab.Models;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
	private readonly AppDbContext _context;

	public StudentsController(AppDbContext context)
	{
		_context = context;
	}

    // GET: api/Students?limit=10&sort=NAME&offset=0&minid=1&maxid=100&like=John&columns=Id,Phone&globalike=John
    [HttpGet]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStudents(
	[FromQuery] int? limit,
    [FromQuery] int? offset,
    [FromQuery] int? minid,
    [FromQuery] int? maxid,
    [FromQuery] string sort = null, // делаем необязательным с null значением
    [FromQuery] string like = null, // делаем необязательным с null значением
    [FromQuery] string columns = null, // делаем необязательным с null значением
    [FromQuery] string globalike = null // делаем необязательным с null значением	
	)

    {
		var students = _context.Students.AsQueryable();

		if (minid.HasValue)
			students = students.Where(s => s.Id >= minid);
		if (maxid.HasValue)
			students = students.Where(s => s.Id <= maxid);
		if (!string.IsNullOrEmpty(like))
			students = students.Where(s => s.Name.Contains(like));
		if (!string.IsNullOrEmpty(globalike))
			students = students.Where(s => $"{s.Id}{s.Name}{s.Phone}".Contains(globalike));

		if (!string.IsNullOrEmpty(sort) && sort.Equals("NAME", StringComparison.OrdinalIgnoreCase))
			students = students.OrderBy(s => s.Name);
		else
			students = students.OrderBy(s => s.Id);

		if (offset.HasValue)
			students = students.Skip(offset.Value);
		if (limit.HasValue)
			students = students.Take(limit.Value);

		var selectedColumns = !string.IsNullOrEmpty(columns) ? columns.Split(',') : null;

		var result = students.Select(s => new
		{
			Id = selectedColumns == null || selectedColumns.Contains("Id") ? s.Id : 0,
			Name = selectedColumns == null || selectedColumns.Contains("Name") ? s.Name : null,
			Phone = selectedColumns == null || selectedColumns.Contains("Phone") ? s.Phone : null,
			Links = new[] {
				new { rel = "self", href = Url.Action("GetStudent", new { id = s.Id }) }
			}
		});

		return Ok(result);
	}

    // GET: api/Students/5
    [HttpGet("{id}")]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(int id)
	{
		var student = await _context.Students.FindAsync(id);

		if (student == null)
			return NotFound(new { error = "Student not found", links = new[] { new { rel = "list", href = Url.Action("GetStudents") } } });

		return Ok(new
		{
			student.Id,
			student.Name,
			student.Phone,
			Links = new[] {
				new { rel = "self", href = Url.Action("GetStudent", new { id = student.Id }) },
				new { rel = "list", href = Url.Action("GetStudents") }
			}
		});
	}

	// POST: api/Students
	[HttpPost]
	public async Task<IActionResult> CreateStudent([FromBody] Student student)
	{
		_context.Students.Add(student);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
	}

	// PUT: api/Students/5
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student student)
	{
		if (id != student.Id)
			return BadRequest(new { error = "ID mismatch", links = new[] { new { rel = "self", href = Url.Action("GetStudent", new { id }) } } });

		_context.Entry(student).State = EntityState.Modified;
		await _context.SaveChangesAsync();

		return NoContent();
	}

	// DELETE: api/Students/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteStudent(int id)
	{
		var student = await _context.Students.FindAsync(id);
		if (student == null)
			return NotFound(new { error = "Student not found", links = new[] { new { rel = "list", href = Url.Action("GetStudents") } } });

		_context.Students.Remove(student);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}
