using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AbbaXpress.API.Data;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfiguracionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE,ADMIN_SUCURSAL")]
        public async Task<ActionResult<ConfiguracionSucursal>> GetConfiguracion([FromQuery] int? sucursalId)
        {
            var (esValido, targetId, errorResult) = await ValidarAccesoSucursalAsync(sucursalId);
            if (!esValido) return errorResult!;

            var targetSucursal = await _context.Sucursales.FindAsync(targetId);
            int sucursalConfigId = targetId;

            // Sedes propias comparten la configuración central de la matriz
            if (targetSucursal != null && targetSucursal.TipoSucursal == "PROPIA")
            {
                sucursalConfigId = 1;
            }

            var config = await _context.Configuraciones
                .Include(c => c.Sucursal)
                .FirstOrDefaultAsync(c => c.SucursalId == sucursalConfigId);

            if (config == null)
            {
                config = new ConfiguracionSucursal
                {
                    SucursalId = sucursalConfigId,
                    TipoCambioNIO = 36.6243m,
                    TarifaAereoGeneral = 7.00m,
                    TarifaMaritimoGeneral = 4.00m,
                    TarifaCelularFija = 35.00m,
                    TarifaTvMaritimo = 3.50m,
                    TarifaTvAereo = 7.50m,
                    CostoProveedorAereo = 3.80m,
                    CostoProveedorMaritimo = 1.50m,
                    UltimaModificacion = DateTime.UtcNow
                };
                _context.Configuraciones.Add(config);
                await _context.SaveChangesAsync();
            }

            return Ok(config);
        }

        [HttpPut]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE,ADMIN_SUCURSAL")]
        public async Task<IActionResult> UpdateConfiguracion([FromQuery] int sucursalId, [FromBody] ConfiguracionSucursal dto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var targetSucursal = await _context.Sucursales.FindAsync(sucursalId);
            if (targetSucursal == null) return NotFound(new { message = "Sucursal no encontrada" });

            // Bloqueo: Casa Matriz no puede fijar las tarifas de venta de franquicias B2B autónomas
            if (targetSucursal.TipoSucursal == "FRANQUICIA_B2B" && miSucursalId != targetSucursal.Id && userRole != "SUPER_ADMIN")
            {
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new { message = "Las tarifas de venta de una franquicia B2B son autónomas y solo pueden ser modificadas por su propia administración." });
            }

            int sucursalConfigId = targetSucursal.TipoSucursal == "PROPIA" ? 1 : sucursalId;

            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == sucursalConfigId);
            if (config == null)
            {
                config = new ConfiguracionSucursal { SucursalId = sucursalConfigId };
                _context.Configuraciones.Add(config);
            }

            config.TipoCambioNIO = dto.TipoCambioNIO;
            config.TarifaAereoGeneral = dto.TarifaAereoGeneral;
            config.TarifaMaritimoGeneral = dto.TarifaMaritimoGeneral;
            config.TarifaCelularFija = dto.TarifaCelularFija;
            config.TarifaTvMaritimo = dto.TarifaTvMaritimo;
            config.TarifaTvAereo = dto.TarifaTvAereo;
            config.CostoProveedorAereo = dto.CostoProveedorAereo;
            config.CostoProveedorMaritimo = dto.CostoProveedorMaritimo;
            config.UltimaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(config);
        }

        private async Task<(bool EsValido, int TargetId, ActionResult? ErrorResult)> ValidarAccesoSucursalAsync(int? sucursalIdParam)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;
            int targetId = sucursalIdParam ?? miSucursalId;

            if (userRole == "SUPER_ADMIN")
                return (true, targetId, null);

            var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
            var targetSucursal = await _context.Sucursales.FindAsync(targetId);

            if (miSucursal == null || targetSucursal == null)
                return (false, targetId, NotFound());

            if (miSucursal.TipoSucursal == "PROPIA" && targetSucursal.TipoSucursal != "PROPIA")
                return (false, targetId, Forbid());

            if (miSucursal.TipoSucursal == "FRANQUICIA_B2B" && targetId != miSucursalId)
                return (false, targetId, Forbid());

            return (true, targetId, null);
        }
    }
}