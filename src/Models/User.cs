namespace CapsuleApi.src.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Plan { get; set; } // "Free" or "Premium"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ProfileImagePath { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Capsule> Capsules { get; set; }
}
