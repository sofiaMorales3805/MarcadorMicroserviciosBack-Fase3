using Microsoft.AspNetCore.Mvc;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Services.Interfaces;
using MarcadorFaseIIApi.Models.DTOs;
using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Repositories;

namespace MarcadorFaseIIApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles); // devolverá lista con Id y Name
    }
}
