using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if (await _repo.GetByEmailAsync(user.Email) != null)
            return BadRequest("User already exists.");

        await _repo.AddAsync(user);
        return Ok("Registered successfully.");
    }
}
