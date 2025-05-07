using System.ComponentModel.DataAnnotations;

namespace CapsuleApi.src.Models.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "O primeiro nome é obrigatório.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de e-mail é obrigatória.")]
    [Compare(nameof(Email), ErrorMessage = "O e-mail e a confirmação não coincidem.")]
    public string ConfirmEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [Compare(nameof(Password), ErrorMessage = "A senha e a confirmação não coincidem.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
