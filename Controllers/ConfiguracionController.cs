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
            var (esValido, targetId, errorResult) = ValidarAccesoSucursal(sucursalId);
            if (!esValido) return errorResult!;

            // Sincronización Managua: Si el objetivo es Doral (2), unificamos al registro central de Bolonia (1)
            int sucursalConfigId = targetId == 2 ? 1 : targetId;

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
        public async Task<IActionResult> UpdateConfiguracion([FromBody] ConfiguracionSucursal model, [FromQuery] int? sucursalId)
        {
            var (esValido, targetId, errorResult) = ValidarAccesoSucursal(sucursalId);
            if (!esValido) return errorResult!;

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            // Sincronización Managua al actualizar
            int sucursalConfigId = targetId == 2 ? 1 : targetId;

            var config = await _context.Configuraciones
                .FirstOrDefaultAsync(c => c.SucursalId == sucursalConfigId);

            bool esNuevo = false;
            if (config == null)
            {
                config = new ConfiguracionSucursal { SucursalId = sucursalConfigId };
                _context.Configuraciones.Add(config);
                esNuevo = true;
            }

            config.TipoCambioNIO = model.TipoCambioNIO;
            config.TarifaAereoGeneral = model.TarifaAereoGeneral;
            config.TarifaMaritimoGeneral = model.TarifaMaritimoGeneral;
            config.TarifaCelularFija = model.TarifaCelularFija;
            config.TarifaTvMaritimo = model.TarifaTvMaritimo;
            config.TarifaTvAereo = model.TarifaTvAereo;
            config.CostoProveedorAereo = model.CostoProveedorAereo;
            config.CostoProveedorMaritimo = model.CostoProveedorMaritimo;
            config.UltimaModificacion = DateTime.UtcNow;

            // Registro de auditoría por actualización de tarifas/configuración
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = sucursalConfigId,
                UsuarioId = usuarioId,
                Accion = esNuevo ? "CREACION" : "MODIFICACION",
                Modulo = "CONFIGURACION",
                Descripcion = $"El usuario {nombreUsuario} actualizó los parámetros y tarifas globales (T/C: {config.TipoCambioNIO} NIO, Aéreo: ${config.TarifaAereoGeneral}, Marítimo: ${config.TarifaMaritimoGeneral}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(config);
        }

        // Método reutilizable de seguridad de alcance
        private (bool EsValido, int TargetId, ActionResult? ErrorResult) ValidarAccesoSucursal(int? sucursalIdParam)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;
            int targetId = sucursalIdParam ?? miSucursalId;

            if (userRole == "SUPER_ADMIN")
            {
                return (true, targetId, null);
            }

            // Regla Managua: Sucursales 1 y 2 (Bolonia y Doral)
            bool soyManagua = miSucursalId == 1 || miSucursalId == 2;
            bool targetEsManagua = targetId == 1 || targetId == 2;

            // Regla León: Sucursal 3
            bool soyLeon = miSucursalId == 3;
            bool targetEsLeon = targetId == 3;

            if (soyManagua && !targetEsManagua)
                return (false, targetId, Forbid());

            if (soyLeon && !targetEsLeon)
                return (false, targetId, Forbid());

            return (true, targetId, null);
        }
    }
}