using CapsuleApi.src.Models.DTOs;
using CapsuleApi.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapsuleApi.src.Controllers;

[ApiController]
[Route("auth")]
public class AuthController: ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var sucess = await _authService.RegisterUserAsync(request);

        if (!sucess)
            return BadRequest("Falha no cadastro. Verifique os dados informados.");

        return Ok("Usuário registrado com sucesso!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if(result == null)
            return Unauthorized("Credenciais inválidas");

        return Ok(result);
    }
}
