using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Verifique as credenciais do usuário
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Crie uma identidade para o usuário autenticado
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Faça a autenticação do usuário
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = false
                    });

                return Ok(new { Id = user.Id }); // Retorne o ID do usuário na resposta
            }

            // Credenciais inválidas
            return Unauthorized();
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Efetue o logout do usuário
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string password, [FromQuery] string email)
        {
            if (UserExists(username))
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }






        [HttpGet("getusername")]
        [Authorize]
        public IActionResult GetUsername()
        {
            var username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            return Ok(new { username });
        }




        private bool UserExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

    }
}
