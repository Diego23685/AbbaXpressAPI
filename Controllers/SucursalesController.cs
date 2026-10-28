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
    public class SucursalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/sucursales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetSucursales([FromQuery] bool? incluirInactivas)
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;
            var rol = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            var query = _context.Sucursales.AsQueryable();

            if (incluirInactivas != true || rol != "SUPER_ADMIN")
            {
                query = query.Where(s => s.Activa);
            }

            if (rol != "SUPER_ADMIN" && miSucursalId == 3)
            {
                query = query.Where(s => s.Id == miSucursalId);
            }

            return Ok(await query.OrderBy(s => s.Id).ToListAsync());
        }

        // GET: api/sucursales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sucursal>> GetSucursal(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound(new { message = "Sucursal no encontrada" });

            return Ok(sucursal);
        }

        // POST: api/sucursales (Exclusivo Super Admin)
        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<ActionResult<Sucursal>> CreateSucursal([FromBody] SucursalCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            var nuevaSucursal = new Sucursal
            {
                Nombre = dto.Nombre.Trim(),
                Ciudad = dto.Ciudad.Trim(),
                Direccion = dto.Direccion?.Trim(),
                Telefono = dto.Telefono?.Trim(),
                TipoSucursal = dto.TipoSucursal.ToUpper(),
                Activa = true,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Sucursales.Add(nuevaSucursal);
            await _context.SaveChangesAsync();

            // Si es FRANQUICIA_B2B, inicializar automáticamente su registro de tarifas en Configuraciones
            if (nuevaSucursal.TipoSucursal == "FRANQUICIA_B2B")
            {
                var configPlantilla = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == 1);

                var nuevaConfig = new ConfiguracionSucursal
                {
                    SucursalId = nuevaSucursal.Id,
                    TipoCambioNIO = configPlantilla?.TipoCambioNIO ?? 36.6243m,
                    TarifaAereoGeneral = configPlantilla?.TarifaAereoGeneral ?? 7.50m,
                    TarifaMaritimoGeneral = configPlantilla?.TarifaMaritimoGeneral ?? 4.50m,
                    TarifaCelularFija = configPlantilla?.TarifaCelularFija ?? 35.00m,
                    TarifaTvMaritimo = configPlantilla?.TarifaTvMaritimo ?? 4.00m,
                    TarifaTvAereo = configPlantilla?.TarifaTvAereo ?? 8.00m,
                    CostoProveedorAereo = configPlantilla?.CostoProveedorAereo ?? 5.50m,
                    CostoProveedorMaritimo = configPlantilla?.CostoProveedorMaritimo ?? 3.00m,
                    UltimaModificacion = DateTime.UtcNow
                };

                _context.Configuraciones.Add(nuevaConfig);

                // Darla de alta automáticamente como cliente SUBAGENCIA para que Managua pueda seleccionarla al recepcionar
                _context.Clientes.Add(new Cliente
                {
                    SucursalId = 1, // Pertenece a la cartera de Managua como cliente B2B
                    Nombre = $"Subagencia {nuevaSucursal.Nombre}",
                    CodigoPais = "+505",
                    Telefono = nuevaSucursal.Telefono ?? "00000000",
                    Direccion = nuevaSucursal.Direccion,
                    TarifaAereo = nuevaConfig.CostoProveedorAereo,
                    TarifaMaritimo = nuevaConfig.CostoProveedorMaritimo,
                    TipoCliente = "SUBAGENCIA",
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = 1,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "SUCURSALES",
                Descripcion = $"El usuario {nombreUsuario} dio de alta la nueva sucursal '{nuevaSucursal.Nombre}' ({nuevaSucursal.TipoSucursal}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSucursal), new { id = nuevaSucursal.Id }, nuevaSucursal);
        }

        // PUT: api/sucursales/5 (Exclusivo Super Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> UpdateSucursal(int id, [FromBody] SucursalUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null) return NotFound(new { message = "Sucursal no encontrada" });

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            sucursal.Nombre = dto.Nombre.Trim();
            sucursal.Ciudad = dto.Ciudad.Trim();
            sucursal.Direccion = dto.Direccion?.Trim();
            sucursal.Telefono = dto.Telefono?.Trim();
            sucursal.TipoSucursal = dto.TipoSucursal.ToUpper();

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = 1,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "SUCURSALES",
                Descripcion = $"El usuario {nombreUsuario} actualizó la información de la sucursal '{sucursal.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(sucursal);
        }

        // PUT: api/sucursales/5/toggle-estado (Borrado lógico suave)
        [HttpPut("{id}/toggle-estado")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            if (id == 1)
                return BadRequest(new { message = "No es posible desactivar la sucursal central principal." });

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null) return NotFound(new { message = "Sucursal no encontrada" });

            sucursal.Activa = !sucursal.Activa;

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = 1,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "SUCURSALES",
                Descripcion = $"El usuario {nombreUsuario} cambió el estado de '{sucursal.Nombre}' a {(sucursal.Activa ? "ACTIVA" : "INACTIVA")}.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Sucursal {(sucursal.Activa ? "activada" : "desactivada")} correctamente." });
        }
    }
}