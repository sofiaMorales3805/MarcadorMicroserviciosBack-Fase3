using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Data;
using MarcadorFaseIIApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MarcadorFaseIIApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MarcadorDbContext _context;
    public UserRepository(MarcadorDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Busca un usuario en la base de datos y retorna la coincidencia junto con su rol.
    /// </summary>
    /// <param name="username">Nombre de usuario (string requerido).</param>
    /// <param name="password">Contraseña del usuario (string requerida).</param>
    /// <returns>El usuario encontrado con su rol o <c>null</c> si no existe.</returns>
    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id); // Clave primaria -> FindAsync

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByUsernameWithRoleAsync(string username) =>
        await _context.Users
                      .Include(u => u.Role)
                      .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.ToListAsync();

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
