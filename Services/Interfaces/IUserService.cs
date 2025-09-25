using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task DeleteAsync(int id);
    }
}
