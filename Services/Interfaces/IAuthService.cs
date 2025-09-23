using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
    Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request);
}
