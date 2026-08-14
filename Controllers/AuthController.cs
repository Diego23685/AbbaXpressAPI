using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AbbaXpress.API.Data;
using AbbaXpress.API.DTOs;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var user = await _context.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (user == null || !user.Activo)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });

            bool passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordValido)
                return Unauthorized(new { message = "Credenciales incorrectas." });

            var token = GenerarJwtToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Id = user.Id,
                Nombre = user.Nombre,
                Username = user.Username,
                Rol = user.Rol,
                SucursalId = user.SucursalId,
                SucursalNombre = user.Sucursal?.Nombre ?? "Sin Asignar"
            });
        }

        private string GenerarJwtToken(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Rol),
                new Claim("SucursalId", user.SucursalId.ToString()),
                new Claim("SucursalNombre", user.Sucursal?.Nombre ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}