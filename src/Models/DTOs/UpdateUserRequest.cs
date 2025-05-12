using System.ComponentModel.DataAnnotations;

namespace CapsuleApi.src.Models.DTOs;

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? ConfirmEmail { get; set; }

    [MinLength(6)]
    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
