using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AbbaXpress.API.Data;
using AbbaXpress.API.DTOs;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SetupController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/setup/seed-admin
        [HttpPost("seed-admin")]
        public async Task<IActionResult> SeedInitialAdmin([FromBody] UsuarioCreateDto dto)
        {
            // 1. Regla de Bloqueo Permanente: Solo se permite si la tabla está totalmente vacía
            bool existenUsuarios = await _context.Usuarios.AnyAsync();
            if (existenUsuarios)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new 
                { 
                    message = "Acceso denegado: El sistema ya ha sido inicializado y cuenta con usuarios registrados." 
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. Garantizar que exista al menos una sucursal base
            var sucursal = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (sucursal == null)
            {
                sucursal = await _context.Sucursales.FirstOrDefaultAsync();
                if (sucursal == null)
                {
                    sucursal = new Sucursal
                    {
                        Id = 1,
                        Nombre = "Sucursal Bolonia - Central",
                        Ciudad = "Managua",
                        Direccion = "Bolonia, de Plaza España 1c al oeste",
                        Telefono = "+505 2222-1111",
                        TipoSucursal = "PROPIA",
                        Activa = true,
                        FechaRegistro = DateTime.UtcNow
                    };
                    _context.Sucursales.Add(sucursal);
                    await _context.SaveChangesAsync();
                }
            }

            // 3. Crear el usuario Administrador Inicial
            var adminUser = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Username = dto.Username.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "SUPER_ADMIN", // Forzado a Super Admin por seguridad de arranque
                SucursalId = sucursal.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(adminUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Usuario Super Admin inicial creado exitosamente. El endpoint ha quedado bloqueado permanentemente.",
                username = adminUser.Username,
                rol = adminUser.Rol,
                sucursalId = adminUser.SucursalId
            });
        }
    }
}