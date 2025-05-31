using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly BudgetDbContext _context;

    public UserRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        user.EncryptedPassword = PasswordHasher.HashPassword(user.EncryptedPassword);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}