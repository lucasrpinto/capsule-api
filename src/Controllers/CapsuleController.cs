using CapsuleApi.src.Data;
using CapsuleApi.src.Models;
using CapsuleApi.src.Models.DTOs;
using CapsuleApi.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapsuleApi.src.Controllers;

[Authorize]
[ApiController]
[Route("capsules")]
public class CapsuleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly AuthService _authService;
    private readonly FileStorageService _fileStorage;

    public CapsuleController(AppDbContext context, IWebHostEnvironment env, AuthService authService, FileStorageService fileStorage)
    {
        _context = context; 
        _env = env;
        _authService = authService;
        _fileStorage = fileStorage;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCapsule([FromForm] CapsuleRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var plan = User.FindFirst("plan")?.Value;

        if(userId == null || plan == null) 
            return Unauthorized(new {message = "Token invalido ou ausente."});

        var oneYearFromNow = DateTime.UtcNow.AddYears(1);

        if(plan == "Free")
        {
            if (request.Files != null && request.Files.Any())
                return BadRequest(new { message = "O plano Free não permite envio de arquivos." });

            if (request.OpenDate > oneYearFromNow)
                return BadRequest(new { message = "O plano Free permite cápsulas apenas com abertura em até 1 ano." });
        }

        var capsule = new Capsule
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Message = request.Message,
            OpenDate = request.OpenDate,
            CreatedAt = DateTime.UtcNow,
            PlanType = plan,
            UserId = Guid.Parse(userId)
        };

        _context.Capsules.Add(capsule);

        if (plan == "Premium" && request.Files != null)
        {
            var fileList = new List<CapsuleFile>();

            foreach (var file in request.Files)
            {
                var fileUrl = await _fileStorage.UploadFileAsync(file, "capsules");

                fileList.Add(new CapsuleFile
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FilePath = fileUrl,
                    FileType = file.ContentType,
                    UploadedAt = DateTime.UtcNow,
                    CapsuleId = capsule.Id
                });
            }

            _context.Files.AddRange(fileList); // Adiciona os arquivos ao contexto
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Cápsula criada com sucesso!" });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyCapsules()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new { message = "Token invalido." });

        var guid = Guid.Parse(userId);

        var capsules = await _context.Capsules
            .Where(c => c.UserId == guid)
            .Include(c => c.Files)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var result = capsules.Select(c => new
        {
            c.Id,
            c.Title,
            c.Message,
            OpenDate = c.OpenDate.ToString("yyyy-MM-dd"),
            c.CreatedAt,
            c.PlanType,
            Files = c.Files.Select(f => new
            {
                f.FileName,
                f.FileType,
                f.FilePath // aqui vai o link completo do R2
            })
        });

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCapsule(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new { message = "Token inválido" });

        var guid = Guid.Parse(userId);

        var capsule = await _context.Capsules
            .Include(c => c.Files)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == guid);

        if (capsule == null)
            return NotFound(new { message = "Cápusla não encontrada."});

        foreach (var file in capsule.Files)
        {
            var fileUrl = file.FilePath;

            if (!string.IsNullOrEmpty(fileUrl))
            {
                // extrai a key removendo o inicio da URL
                var key = fileUrl.Replace($"{_fileStorage.Endpoint}/{_fileStorage.BucketName}/", "");
                await _fileStorage.DeleteFileAsync(key);
            }
        }

        _context.Capsules.Remove(capsule);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cápusla deletada com sucesso."});
    }

    [Authorize]
    [HttpPut]
    [Route("users")]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new { message = "Token inválido" });

        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado" });

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        // E-mail: só altera se ambos forem preenchidos e iguais
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !string.IsNullOrWhiteSpace(request.ConfirmEmail) &&
            request.Email == request.ConfirmEmail)
        {
            var existingEmail = await _context.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != user.Id);

            if (existingEmail)
                return BadRequest(new { message = "Este e-mail já está cadastrado." });

            user.Email = request.Email;
        }

        // Senha: só altera se ambas forem preenchidas e iguais
        if (!string.IsNullOrWhiteSpace(request.Password) &&
            !string.IsNullOrWhiteSpace(request.ConfirmPassword) &&
            request.Password == request.ConfirmPassword)
        {
            user.PasswordHash = _authService.HashPassword(request.Password);
        }


        if (request.ProfileImage != null)
        {
            var fileUrl = await _fileStorage.UploadFileAsync(request.ProfileImage, "profiles");
            user.ProfileImagePath = fileUrl;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Dados atualizados com sucesso." });
    }

    [Authorize]
    [HttpGet]
    [Route("users/profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized(new { message = "Token inválido." });

        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        return Ok(new
        {
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            plan = user.Plan,
            profileImageUrl = user.ProfileImagePath // ex: "/profile-images/foto.png"
        });
    }
}
