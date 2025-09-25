using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Services.Interfaces;
using MarcadorFaseIIApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MarcadorFaseIIApi.Services
{
    public class UserService : IUserService
    {
        private readonly MarcadorDbContext _context;

        public UserService(MarcadorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Role).ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
