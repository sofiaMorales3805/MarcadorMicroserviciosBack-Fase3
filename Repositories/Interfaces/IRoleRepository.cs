using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
    }
}
