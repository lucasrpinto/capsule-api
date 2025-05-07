namespace CapsuleApi.src.Models;

public class CapsuleFile
{
    public Guid Id { get; set; }
    public Guid CapsuleId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileType { get; set; } // image, video, audio, document
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Capsule Capsule { get; set; }
}
