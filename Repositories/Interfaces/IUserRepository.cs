using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByUsernameWithRoleAsync(string username); 
        Task AddAsync(User user);
    }
}
