using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs;
using MarcadorFaseIIApi.Repositories.Interfaces;
using MarcadorFaseIIApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MarcadorFaseIIApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _config = config;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
    {
        // Trae el usuario *con* su rol
        var user = await _userRepository.GetByUsernameWithRoleAsync(request.Username);
        if (user == null) return null;

        // Verifica contraseña hasheada
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        // Carga de respaldo por si el rol no vino (defensa extra)
        if (user.Role == null)
        {
            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (role != null) user.Role = role;
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Username = user.Username,
            Role = new RoleDto { Name = user.Role?.Name ?? "User" },
            Token = token
        };
    }

    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
            return null;

        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
            return null;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Password = hashedPassword,
            RoleId = role.Id
        };

        await _userRepository.AddAsync(user);

        return new RegisterResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            RoleName = role.Name,
            Message = "Usuario registrado correctamente"
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourSuperSecretKey123!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roleName = user.Role?.Name ?? "User";

        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };

        var minutes = int.TryParse(jwtSettings["ExpiresInMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(minutes),
        signingCredentials: creds
    );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
