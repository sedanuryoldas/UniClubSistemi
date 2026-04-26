using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Bu email adresi zaten kullanımda.");
        }

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = request.Password, // Gerçek projede hashlenmeli
            Role = "Student"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Kayıt başarıyla tamamlandı!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.PasswordHash != request.Password)
        {
            return BadRequest("Email veya şifre hatalı.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bu_cok_gizli_ve_uzun_bir_anahtar_cumlesi_olmalidir_12345!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = jwt, User = new { user.FullName, user.Email, user.Role } });
    }
}