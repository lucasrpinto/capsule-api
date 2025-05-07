using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CapsuleApi.src.Models.DTOs;

public class CapsuleRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public DateTime OpenDate { get; set; }

    public List<IFormFile>? Files { get; set; }
}
