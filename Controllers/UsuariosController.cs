using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AbbaXpress.API.Data;
using AbbaXpress.API.DTOs;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.Usuarios
                .Include(u => u.Sucursal)
                .AsQueryable();

            if (miSucursalId == 3) // León
            {
                query = query.Where(u => u.SucursalId == 3);
            }
            else // Managua (Bolonia y Doral)
            {
                query = query.Where(u => u.SucursalId == 1 || u.SucursalId == 2);
            }

            var usuarios = await query
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Username,
                    u.Rol,
                    u.SucursalId,
                    SucursalNombre = u.Sucursal != null ? u.Sucursal.Nombre : "Sin Asignar",
                    u.Activo,
                    u.FechaCreacion
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            // Regla de Aislamiento
            bool soyLeon = miSucursalId == 3;
            bool soyManagua = miSucursalId == 1 || miSucursalId == 2;

            if (soyLeon && dto.SucursalId != 3)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permisos para crear usuarios en sucursales de Managua." });

            if (soyManagua && (dto.SucursalId != 1 && dto.SucursalId != 2))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "No tiene permisos para crear usuarios en la sucursal de León." });

            bool existeUsername = await _context.Usuarios
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (existeUsername)
                return BadRequest(new { message = "El nombre de usuario ya está en uso." });

            bool existeSucursal = await _context.Sucursales.AnyAsync(s => s.Id == dto.SucursalId);
            if (!existeSucursal)
                return BadRequest(new { message = "La sucursal seleccionada no existe." });

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Username = dto.Username.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol.ToUpper(),
                SucursalId = dto.SucursalId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);

            // Registro de auditoría por creación
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "USUARIOS",
                Descripcion = $"El usuario {nombreUsuario} creó una nueva cuenta para '{nuevoUsuario.Nombre}' (Usuario: {nuevoUsuario.Username}, Rol: {nuevoUsuario.Rol}, Sucursal ID: {nuevoUsuario.SucursalId}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario '{nuevoUsuario.Username}' creado exitosamente con rol {nuevoUsuario.Rol}." });
        }

        // PUT: api/usuarios/{id} (Actualización de datos y contraseña)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreOperador = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // Validar límites de sede del operador actual
            bool soyLeon = miSucursalId == 3;
            bool soyManagua = miSucursalId == 1 || miSucursalId == 2;

            if (soyLeon && (usuario.SucursalId != 3 || dto.SucursalId != 3))
                return Forbid();

            if (soyManagua && ((usuario.SucursalId != 1 && usuario.SucursalId != 2) || (dto.SucursalId != 1 && dto.SucursalId != 2)))
                return Forbid();

            usuario.Nombre = dto.Nombre.Trim();
            usuario.Rol = dto.Rol.ToUpper();
            usuario.SucursalId = dto.SucursalId;

            bool passwordModificado = false;
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password.Trim());
                passwordModificado = true;
            }

            // Registro de auditoría por actualización
            string detalle = passwordModificado 
                ? $"El usuario {nombreOperador} actualizó los datos y la contraseña de '{usuario.Nombre}' (Usuario: {usuario.Username})."
                : $"El usuario {nombreOperador} actualizó los datos de '{usuario.Nombre}' (Usuario: {usuario.Username}, Rol: {usuario.Rol}).";

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "USUARIOS",
                Descripcion = detalle,
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario '{usuario.Username}' actualizado con éxito." });
        }

        // PUT: api/usuarios/{id}/toggle-estado
        [HttpPut("{id}/toggle-estado")]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (usuario.Username == "admin")
                return BadRequest(new { message = "No se puede desactivar la cuenta principal de Super Admin." });

            bool soyLeon = miSucursalId == 3;
            bool soyManagua = miSucursalId == 1 || miSucursalId == 2;

            if (soyLeon && usuario.SucursalId != 3)
                return Forbid();

            if (soyManagua && (usuario.SucursalId != 1 && usuario.SucursalId != 2))
                return Forbid();

            usuario.Activo = !usuario.Activo;
            string estadoTexto = usuario.Activo ? "activó" : "desactivó";

            // Registro de auditoría por cambio de estado
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "USUARIOS",
                Descripcion = $"El usuario {nombreUsuario} {estadoTexto} la cuenta de '{usuario.Nombre}' (Usuario: {usuario.Username}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario {(usuario.Activo ? "activado" : "desactivado")} con éxito." });
        }
    }
}