namespace CapsuleApi.src.Models;

public class Capsule
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PlanType { get; set; } // "Free" ou "Premium"
    public bool IsOpened { get; set; } = false;

    public ICollection<CapsuleFile> Files { get; set; }
}
