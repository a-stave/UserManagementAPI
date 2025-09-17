using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // Get users by Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        if (id <= 0 || id > 9999999) return BadRequest("Invalid ID.");
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("User not found.");
        return Ok(user);
    }

    // Get all users, paginated to reduce server load and DOS vulnerability.
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(users);
    }

    // Create user accounts
    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        if (!ModelState.IsValid || user.Id <= 0 || user.Id > 9999999 || string.IsNullOrWhiteSpace(user.Name))
            return BadRequest("Invalid input.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Edit user accounts
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        if (id != updatedUser.Id) return BadRequest("ID mismatch.");
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("User not found.");

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Delete user accounts
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}