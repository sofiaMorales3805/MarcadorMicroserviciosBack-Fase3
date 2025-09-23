using Microsoft.AspNetCore.Mvc;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Services.Interfaces;
using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.AuthenticateAsync(request);
        if (response == null)
            return Unauthorized("Invalid username or password");

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registro([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result == null)
            return BadRequest(new { message = "No se pudo registrar el usuario. Verifica el rol o si el usuario ya existe." });

         return Ok(result); 
    }

}
