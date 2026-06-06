using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotatnikAPI.Data;
using NotatnikAPI.DTOs;
using NotatnikAPI.Models;
using NotatnikAPI.Services;

namespace NotatnikAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: register
        [HttpPost("/register")]
        public async Task<IActionResult> Register(CreateUser input)
        {
            if (await _context.Users.AnyAsync(u => u.Email == input.Email))
            {
                return BadRequest("Użytkownik o wskazanym adresie e-mail już istnieje.");
            }

            var user = new User
            {
                Email = input.Email,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            input.Password = "";
            return Created("", input);
        }

        // POST: login
        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginUser input)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == input.Email);
            if (user == null)
            {
                return BadRequest("Nieprawidłowy e-mail lub hasło");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, input.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return BadRequest("Nieprawidłowy e-mail lub hasło");
            }

            return new JsonResult(_jwtService.CreateToken(user));
        }
    }
}
