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
    public class AuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/auditoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLogs(
            [FromQuery] int? sucursalId,
            [FromQuery] string? modulo,
            [FromQuery] string? busqueda)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            var query = _context.LogsAuditoria
                .Include(l => l.Usuario)
                .Include(l => l.Sucursal)
                .Where(l => targetIds.Contains(l.SucursalId))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(modulo))
                query = query.Where(l => l.Modulo == modulo.ToUpper());

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.ToLower().Trim();
                query = query.Where(l => 
                    l.Descripcion.ToLower().Contains(term) ||
                    l.Accion.ToLower().Contains(term) ||
                    (l.Usuario != null && l.Usuario.Nombre.ToLower().Contains(term)));
            }

            var lista = await query
                .OrderByDescending(l => l.FechaMovimiento)
                .Select(l => new {
                    l.Id,
                    Sucursal = l.Sucursal != null ? l.Sucursal.Nombre : "Central",
                    Usuario = l.Usuario != null ? l.Usuario.Nombre : "Sistema",
                    l.Accion,
                    l.Modulo,
                    l.Descripcion,
                    l.FechaMovimiento
                })
                .Take(200) // Límite de rendimiento para el historial reciente
                .ToListAsync();

            return Ok(lista);
        }

        private (bool EsValido, List<int> TargetIds, ActionResult? ErrorResult) ValidarAlcanceSucursales(int? sucursalIdParam)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            bool soyLeon = miSucursalId == 3;
            bool soyManagua = miSucursalId == 1 || miSucursalId == 2;

            if (soyLeon)
            {
                if (sucursalIdParam.HasValue && sucursalIdParam.Value != 3)
                    return (false, new List<int>(), Forbid());

                return (true, new List<int> { 3 }, null);
            }

            if (soyManagua)
            {
                if (sucursalIdParam.HasValue)
                {
                    if (sucursalIdParam.Value != 1 && sucursalIdParam.Value != 2)
                        return (false, new List<int>(), Forbid());

                    return (true, new List<int> { sucursalIdParam.Value }, null);
                }

                return (true, new List<int> { 1, 2 }, null);
            }

            if (userRole == "SUPER_ADMIN")
            {
                if (sucursalIdParam.HasValue)
                    return (true, new List<int> { sucursalIdParam.Value }, null);

                return (true, new List<int> { 1, 2, 3 }, null);
            }

            return (true, new List<int> { miSucursalId }, null);
        }
    }
}