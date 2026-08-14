using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AbbaXpress.API.Data;
using AbbaXpress.API.DTOs;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientes([FromQuery] string? busqueda)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.Clientes.Where(c => c.Activo).AsQueryable();

            if (userRole == "SUPER_ADMIN")
            {
                // El Super Admin puede ver todas las sedes
            }
            else
            {
                query = query.Where(c => c.SucursalId == miSucursalId);
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.ToLower().Trim();
                query = query.Where(c => 
                    c.Nombre.ToLower().Contains(term) || 
                    c.Telefono.Contains(term) || 
                    (c.Email != null && c.Email.ToLower().Contains(term)));
            }

            var lista = await query
                .OrderByDescending(c => c.Id)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    CodigoPais = c.CodigoPais,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    TarifaAereo = c.TarifaAereo,
                    TarifaMaritimo = c.TarifaMaritimo,
                    Direccion = c.Direccion,
                    TipoCliente = c.TipoCliente,
                    Activo = c.Activo,
                    FechaRegistro = c.FechaRegistro
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null || !cliente.Activo)
                return NotFound(new { message = "Cliente no encontrado" });

            if (userRole != "SUPER_ADMIN" && cliente.SucursalId != miSucursalId) 
                return Forbid();

            return Ok(new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                CodigoPais = cliente.CodigoPais,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                TarifaAereo = cliente.TarifaAereo,
                TarifaMaritimo = cliente.TarifaMaritimo,
                Direccion = cliente.Direccion,
                TipoCliente = cliente.TipoCliente,
                Activo = cliente.Activo,
                FechaRegistro = cliente.FechaRegistro
            });
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente([FromBody] ClienteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            var nuevoCliente = new Cliente
            {
                SucursalId = miSucursalId,
                Nombre = dto.Nombre.Trim(),
                CodigoPais = dto.CodigoPais,
                Telefono = dto.Telefono.Trim(),
                Email = dto.Email?.Trim(),
                TarifaAereo = dto.TarifaAereo,
                TarifaMaritimo = dto.TarifaMaritimo,
                Direccion = dto.Direccion?.Trim(),
                TipoCliente = dto.TipoCliente,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Clientes.Add(nuevoCliente);

            // Registro de auditoría por creación
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} registró al nuevo cliente '{nuevoCliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var response = new ClienteResponseDto
            {
                Id = nuevoCliente.Id,
                Nombre = nuevoCliente.Nombre,
                CodigoPais = nuevoCliente.CodigoPais,
                Telefono = nuevoCliente.Telefono,
                Email = nuevoCliente.Email,
                TarifaAereo = nuevoCliente.TarifaAereo,
                TarifaMaritimo = nuevoCliente.TarifaMaritimo,
                Direccion = nuevoCliente.Direccion,
                TipoCliente = nuevoCliente.TipoCliente,
                Activo = nuevoCliente.Activo,
                FechaRegistro = nuevoCliente.FechaRegistro
            };

            return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.Id }, response);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDto dto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            if (userRole != "SUPER_ADMIN" && cliente.SucursalId != miSucursalId) 
                return Forbid();

            cliente.Nombre = dto.Nombre.Trim();
            cliente.CodigoPais = dto.CodigoPais;
            cliente.Telefono = dto.Telefono.Trim();
            cliente.Email = dto.Email?.Trim();
            cliente.TarifaAereo = dto.TarifaAereo;
            cliente.TarifaMaritimo = dto.TarifaMaritimo;
            cliente.Direccion = dto.Direccion?.Trim();
            cliente.TipoCliente = dto.TipoCliente;
            cliente.Activo = dto.Activo;

            // Registro de auditoría por modificación
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = cliente.SucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} actualizó la información del cliente '{cliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/clientes/5 (Borrado Lógico)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            if (userRole != "SUPER_ADMIN" && cliente.SucursalId != miSucursalId) 
                return Forbid();

            cliente.Activo = false;

            // Registro de auditoría por eliminación lógica
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = cliente.SucursalId,
                UsuarioId = usuarioId,
                Accion = "ELIMINACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} desactivó al cliente '{cliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}