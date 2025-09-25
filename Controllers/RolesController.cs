using Microsoft.AspNetCore.Mvc;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Repositories.Interfaces;

namespace MarcadorFaseIIApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepo;

        public RolesController(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _roleRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = $"Rol con id {id} no encontrado" });

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            await _roleRepo.AddAsync(role);
            return Ok(role);
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role updatedRole)
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = $"Rol con id {id} no encontrado" });

            role.Name = updatedRole.Name;
            await _roleRepo.UpdateAsync(role);

            return Ok(role);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return NotFound(new { message = $"Rol con id {id} no encontrado" });

            await _roleRepo.DeleteAsync(role);
            return NoContent(); // 204 sin contenido
        }
    }
}
