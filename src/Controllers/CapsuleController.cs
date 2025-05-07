using CapsuleApi.src.Data;
using CapsuleApi.src.Models;
using CapsuleApi.src.Models.DTOs;
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

    public CapsuleController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context; 
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCapsule([FromForm] CapsuleRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var plan = User.FindFirst("plan")?.Value;

        if(userId == null || plan == null) 
            return Unauthorized(new {message = "Token invalido ou ausente."});

        var oneYearFromNow = DateTime.UtcNow.AddDays(1);

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

        if(plan == "Premium" && request.Files != null)
        {
            var fileList = new List<CapsuleFile>();
            var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "updloads");

            if(!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            foreach(var file in request.Files)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                fileList.Add(new CapsuleFile
                {
                    Id= Guid.NewGuid(),
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileType = file.ContentType,
                    UploadedAt = DateTime.UtcNow,
                    CapsuleId = capsule.Id
                });
            }

            _context.Files.AddRange(fileList);
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
                f.FileType
            })
        });

        return Ok(result);
    }
}
