using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/events (Tüm etkinlikleri getir)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.Events.ToListAsync();
    }

    // POST: api/events (Yeni etkinlik ekle)
    [HttpPost]
    public async Task<ActionResult<Event>> PostEvent(Event @event)
    {
        _context.Events.Add(@event);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvents), new { id = @event.Id }, @event);
    }
}