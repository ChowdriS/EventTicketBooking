using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBookingApi.Model;

public class EventImage
{
    [Key]
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Guid EventId { get; set; }
    public Event? Event { get; set; }
}
