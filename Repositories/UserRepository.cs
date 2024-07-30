using Microsoft.EntityFrameworkCore;
using SignalRSample.Data;
using SignalRSample.Models;

namespace SignalRSample.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId); 
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Email == email); 
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
