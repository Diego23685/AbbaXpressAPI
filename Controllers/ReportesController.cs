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
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reportes/general
        [HttpGet("general")]
        public async Task<ActionResult<object>> GetReporteGeneral(
            [FromQuery] int? sucursalId,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            var proformaQuery = _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.SucursalOrigen)
                .Include(p => p.Paquetes)
                .Where(p => targetIds.Contains(p.SucursalOrigenId))
                .AsQueryable();

            var gastoQuery = _context.GastosOperativos
                .Include(g => g.Sucursal)
                .Where(g => targetIds.Contains(g.SucursalId))
                .AsQueryable();

            if (fechaInicio.HasValue)
            {
                proformaQuery = proformaQuery.Where(p => p.FechaRegistro >= fechaInicio.Value);
                gastoQuery = gastoQuery.Where(g => g.FechaGasto >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                proformaQuery = proformaQuery.Where(p => p.FechaRegistro <= fechaFin.Value);
                gastoQuery = gastoQuery.Where(g => g.FechaGasto <= fechaFin.Value);
            }

            var proformas = await proformaQuery.ToListAsync();
            var gastos = await gastoQuery.ToListAsync();

            // Cálculos financieros
            decimal ventasTotalesUSD = proformas.Where(p => p.Estado == "FACTURADO").Sum(p => p.TotalCobradoUSD);
            decimal costosProveedorUSD = proformas.Where(p => p.Estado == "FACTURADO").Sum(p => p.TotalCostoProveedorUSD);
            decimal utilidadBrutaUSD = ventasTotalesUSD - costosProveedorUSD;
            decimal totalGastosOperativosUSD = gastos.Sum(g => g.MontoUSD);
            decimal utilidadNetaUSD = utilidadBrutaUSD - totalGastosOperativosUSD;

            // Ventas por Cliente
            var ventasPorCliente = proformas
                .Where(p => p.Estado == "FACTURADO")
                .GroupBy(p => p.Cliente != null ? p.Cliente.Nombre : "Sin Asignar")
                .Select(g => new {
                    Cliente = g.Key,
                    TotalUSD = g.Sum(p => p.TotalCobradoUSD),
                    CantidadProformas = g.Count()
                })
                .OrderByDescending(x => x.TotalUSD)
                .ToList();

            // Ventas por Sucursal (para Superadmin o sedes consolidadas)
            var ventasPorSucursal = proformas
                .Where(p => p.Estado == "FACTURADO")
                .GroupBy(p => p.SucursalOrigen != null ? p.SucursalOrigen.Nombre : "Sin Sede")
                .Select(g => new {
                    Sucursal = g.Key,
                    TotalUSD = g.Sum(p => p.TotalCobradoUSD),
                    CantidadProformas = g.Count()
                })
                .ToList();

            int primarySucursalId = targetIds.First();
            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == primarySucursalId)
                        ?? new ConfiguracionSucursal { TipoCambioNIO = 36.6243m };

            return Ok(new
            {
                ResumenFinanciero = new
                {
                    VentasTotalesUSD = ventasTotalesUSD,
                    VentasTotalesNIO = ventasTotalesUSD * config.TipoCambioNIO,
                    CostosProveedorUSD = costosProveedorUSD,
                    UtilidadBrutaUSD = utilidadBrutaUSD,
                    GastosOperativosUSD = totalGastosOperativosUSD,
                    UtilidadNetaUSD = utilidadNetaUSD,
                    UtilidadNetaNIO = utilidadNetaUSD * config.TipoCambioNIO,
                    MargenPeridaGanancia = utilidadNetaUSD >= 0 ? "GANANCIA" : "PERDIDA"
                },
                DetalleVentas = proformas.Select(p => new {
                    p.Id,
                    p.NumeroProforma,
                    Cliente = p.Cliente != null ? p.Cliente.Nombre : "Sin Asignar",
                    SucursalOrigen = p.SucursalOrigen != null ? p.SucursalOrigen.Nombre : "",
                    p.Estado,
                    p.TotalLbs,
                    p.TotalCobradoUSD,
                    TotalCobradoNIO = p.TotalCobradoUSD * p.TipoCambioAplicado,
                    p.FechaRegistro
                }),
                VentasPorCliente = ventasPorCliente,
                VentasPorSucursal = ventasPorSucursal,
                GastosRegistrados = gastos.Select(g => new {
                    g.Id,
                    Sucursal = g.Sucursal != null ? g.Sucursal.Nombre : "",
                    g.Categoria,
                    g.Descripcion,
                    g.MontoUSD,
                    g.FechaGasto
                })
            });
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