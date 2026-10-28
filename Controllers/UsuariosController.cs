using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AbbaXpress.API.Data;
using AbbaXpress.API.DTOs;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE,ADMIN_SUCURSAL")]
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
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.Usuarios
                .Include(u => u.Sucursal)
                .AsQueryable();

            if (userRole != "SUPER_ADMIN")
            {
                // Obtenemos dinámicamente el tipo de la sucursal del operador actual
                var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
                string miTipoSucursal = miSucursal?.TipoSucursal ?? "FRANQUICIA_B2B";

                if (miTipoSucursal == "PROPIA")
                {
                    // Un admin de red propia ve a los usuarios de todas las sucursales PROPIAS de la empresa
                    query = query.Where(u => u.Sucursal != null && u.Sucursal.TipoSucursal == "PROPIA");
                }
                else
                {
                    // Una franquicia B2B independiente únicamente ve los usuarios de su propia sede
                    query = query.Where(u => u.SucursalId == miSucursalId);
                }
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

            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var sucursalDestino = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (sucursalDestino == null || !sucursalDestino.Activa)
                return BadRequest(new { message = "La sucursal seleccionada no existe o está inactiva." });

            // Validación dinámica de alcance según el TipoSucursal
            if (userRole != "SUPER_ADMIN")
            {
                var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
                string miTipoSucursal = miSucursal?.TipoSucursal ?? "FRANQUICIA_B2B";

                if (miTipoSucursal == "PROPIA")
                {
                    // Administrador de sucursal propia no puede crear usuarios dentro de franquicias B2B
                    if (sucursalDestino.TipoSucursal != "PROPIA")
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, 
                            new { message = "No tiene permisos para crear usuarios en franquicias o subagencias B2B." });
                    }
                }
                else
                {
                    // Administrador de franquicia B2B solo puede crear usuarios en su propia sucursal
                    if (dto.SucursalId != miSucursalId)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, 
                            new { message = "No tiene permisos para dar de alta usuarios fuera de su sucursal." });
                    }
                }
            }

            bool existeUsername = await _context.Usuarios
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (existeUsername)
                return BadRequest(new { message = "El nombre de usuario ya está en uso." });

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

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "USUARIOS",
                Descripcion = $"El usuario {nombreUsuario} creó una cuenta para '{nuevoUsuario.Nombre}' (Usuario: {nuevoUsuario.Username}, Rol: {nuevoUsuario.Rol}, Sede: {sucursalDestino.Nombre}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario '{nuevoUsuario.Username}' creado exitosamente con rol {nuevoUsuario.Rol}." });
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreOperador = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuario = await _context.Usuarios.Include(u => u.Sucursal).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            var nuevaSucursal = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (nuevaSucursal == null || !nuevaSucursal.Activa)
                return BadRequest(new { message = "La sucursal seleccionada no existe o está inactiva." });

            // Validación dinámica de alcance
            if (userRole != "SUPER_ADMIN")
            {
                var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
                string miTipoSucursal = miSucursal?.TipoSucursal ?? "FRANQUICIA_B2B";

                if (miTipoSucursal == "PROPIA")
                {
                    // No puede editar ni transferir usuarios hacia/desde franquicias B2B
                    if (usuario.Sucursal?.TipoSucursal != "PROPIA" || nuevaSucursal.TipoSucursal != "PROPIA")
                        return Forbid();
                }
                else
                {
                    // Franquicia solo puede modificar a sus propios colaboradores y dejarlos en su sede
                    if (usuario.SucursalId != miSucursalId || dto.SucursalId != miSucursalId)
                        return Forbid();
                }
            }

            usuario.Nombre = dto.Nombre.Trim();
            usuario.Rol = dto.Rol.ToUpper();
            usuario.SucursalId = dto.SucursalId;

            bool passwordModificado = false;
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password.Trim());
                passwordModificado = true;
            }

            string detalle = passwordModificado 
                ? $"El usuario {nombreOperador} actualizó datos y contraseña de '{usuario.Nombre}' (Sede: {nuevaSucursal.Nombre})."
                : $"El usuario {nombreOperador} actualizó datos de '{usuario.Nombre}' (Rol: {usuario.Rol}, Sede: {nuevaSucursal.Nombre}).";

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
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var usuario = await _context.Usuarios.Include(u => u.Sucursal).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (usuario.Username == "admin")
                return BadRequest(new { message = "No se puede desactivar la cuenta principal de Super Admin." });

            if (userRole != "SUPER_ADMIN")
            {
                var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
                string miTipoSucursal = miSucursal?.TipoSucursal ?? "FRANQUICIA_B2B";

                if (miTipoSucursal == "PROPIA")
                {
                    if (usuario.Sucursal?.TipoSucursal != "PROPIA") return Forbid();
                }
                else
                {
                    if (usuario.SucursalId != miSucursalId) return Forbid();
                }
            }

            usuario.Activo = !usuario.Activo;
            string estadoTexto = usuario.Activo ? "activó" : "desactivó";

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "USUARIOS",
                Descripcion = $"El usuario {nombreUsuario} {estadoTexto} la cuenta de '{usuario.Nombre}' ({usuario.Username}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario {(usuario.Activo ? "activado" : "desactivado")} con éxito." });
        }
    }
}