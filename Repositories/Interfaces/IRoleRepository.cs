using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
    }
}
