using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Sadece giriş yapanlar (tokenı olanlar) bu kapıdan geçebilir!
public class SavedEventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SavedEventsController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Etkinliği Kaydet (Öğrenci "Kaydet" butonuna bastığında çalışır)
    [HttpPost("save/{eventId}")]
    public async Task<IActionResult> SaveEvent(int eventId)
    {
        // Token içinden giriş yapan kullanıcının ID'sini çekiyoruz
        var userIdString = User.FindFirst("UserId")?.Value;
        if (userIdString == null) return Unauthorized();

        int userId = int.Parse(userIdString);

        // Zaten kaydedilmiş mi kontrol et?
        var alreadySaved = await _context.SavedEvents
            .AnyAsync(s => s.UserId == userId && s.EventId == eventId);

        if (alreadySaved) return BadRequest("Bu etkinlik zaten listenizde.");

        var savedEvent = new SavedEvent
        {
            UserId = userId,
            EventId = eventId,
            SavedAt = DateTime.Now
        };

        _context.SavedEvents.Add(savedEvent);
        await _context.SaveChangesAsync();

        return Ok("Etkinlik başarıyla kaydedildi.");
    }

    // 2. Kaydettiğim Etkinlikleri Listele (Öğrenci Paneli'nde görünür)
    [HttpGet("my-list")]
    public async Task<IActionResult> GetMySavedEvents()
    {
        var userIdString = User.FindFirst("UserId")?.Value;
        if (userIdString == null) return Unauthorized();

        int userId = int.Parse(userIdString);

        var myList = await _context.SavedEvents
            .Where(s => s.UserId == userId)
            .Include(s => s.Event) // Etkinlik detaylarını da getir (Başlık, Tarih vb.)
            .Select(s => s.Event)
            .ToListAsync();

        return Ok(myList);
    }
}