using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class SavedEvent
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; } // Bu olmazsa hata verir

    public int EventId { get; set; }
    public Event? Event { get; set; } // Hata mesajındaki 'Event' eksikliği burası

    public DateTime SavedAt { get; set; } // Hata mesajındaki 'SavedAt' eksikliği burası
}