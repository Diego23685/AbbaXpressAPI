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
    public class FinanzasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FinanzasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/finanzas/gastos
        [HttpGet("gastos")]
        public async Task<ActionResult<IEnumerable<GastoOperativoResponseDto>>> GetGastos(
            [FromQuery] int? sucursalId,
            [FromQuery] string? categoria,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            var query = _context.GastosOperativos
                .Include(g => g.Sucursal)
                .Include(g => g.Usuario)
                .Where(g => targetIds.Contains(g.SucursalId))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(g => g.Categoria == categoria.ToUpper());

            if (fechaInicio.HasValue)
                query = query.Where(g => g.FechaGasto >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(g => g.FechaGasto <= fechaFin.Value);

            var lista = await query
                .OrderByDescending(g => g.FechaGasto)
                .Select(g => new GastoOperativoResponseDto
                {
                    Id = g.Id,
                    SucursalId = g.SucursalId,
                    SucursalNombre = g.Sucursal != null ? g.Sucursal.Nombre : "Sin Asignar",
                    UsuarioNombre = g.Usuario != null ? g.Usuario.Nombre : "Sistema",
                    Categoria = g.Categoria,
                    Descripcion = g.Descripcion,
                    MontoUSD = g.MontoUSD,
                    MontoNIO = g.MontoUSD * g.TipoCambioAplicado,
                    MetodoPago = g.MetodoPago,
                    NumeroComprobante = g.NumeroComprobante,
                    FechaGasto = g.FechaGasto
                })
                .ToListAsync();

            return Ok(lista);
        }

        // POST: api/finanzas/gastos
        [HttpPost("gastos")]
        public async Task<ActionResult<GastoOperativoResponseDto>> RegistrarGasto([FromBody] GastoOperativoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalIdClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalIdClaim, out int sId) ? sId : 1;
            int sucursalDestinoId = dto.SucursalId ?? miSucursalId;

            // Validar que no registre gastos en otra sede fuera de su jurisdicción
            var (esValido, _, errorResult) = ValidarAlcanceSucursales(sucursalDestinoId);
            if (!esValido) return errorResult!;

            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == sucursalDestinoId)
                        ?? new ConfiguracionSucursal { SucursalId = sucursalDestinoId };

            var nuevoGasto = new GastoOperativo
            {
                SucursalId = sucursalDestinoId,
                UsuarioId = usuarioId,
                Categoria = dto.Categoria.ToUpper(),
                Descripcion = dto.Descripcion,
                MontoUSD = dto.MontoUSD,
                TipoCambioAplicado = dto.TipoCambio > 0 ? dto.TipoCambio : config.TipoCambioNIO,
                MetodoPago = dto.MetodoPago,
                NumeroComprobante = dto.NumeroComprobante,
                FechaGasto = dto.FechaGasto ?? DateTime.UtcNow
            };

            _context.GastosOperativos.Add(nuevoGasto);

            // Registro de auditoría por creación de gasto operativo
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = sucursalDestinoId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "GASTOS",
                Descripcion = $"El usuario {nombreUsuario} registró un gasto de ${dto.MontoUSD:F2} USD ({dto.Categoria.ToUpper()}) - \"{dto.Descripcion}\".",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Gasto registrado exitosamente", gastoId = nuevoGasto.Id });
        }

        // GET: api/finanzas/balance-utilidades
        [HttpGet("balance-utilidades")]
        public async Task<ActionResult<ResumenFinancieroDto>> GetBalanceUtilidades([FromQuery] int? sucursalId)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            int primarySucursalId = targetIds.First();
            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == primarySucursalId) 
                        ?? new ConfiguracionSucursal { SucursalId = primarySucursalId };

            var proformasFacturadas = await _context.Proformas
                .Where(p => p.Estado == "FACTURADO" && targetIds.Contains(p.SucursalOrigenId))
                .ToListAsync();

            var gastosRegistrados = await _context.GastosOperativos
                .Where(g => targetIds.Contains(g.SucursalId))
                .ToListAsync();

            decimal ingresosTotalesUSD = proformasFacturadas.Sum(p => p.TotalCobradoUSD);
            decimal costosProveedorUSD = proformasFacturadas.Sum(p => p.TotalCostoProveedorUSD);
            decimal utilidadBrutaUSD = ingresosTotalesUSD - costosProveedorUSD;

            decimal totalGastosUSD = gastosRegistrados.Sum(g => g.MontoUSD);
            decimal utilidadNetaUSD = utilidadBrutaUSD - totalGastosUSD;

            var desglose = gastosRegistrados
                .GroupBy(g => g.Categoria)
                .ToDictionary(k => k.Key, v => v.Sum(g => g.MontoUSD));

            return Ok(new ResumenFinancieroDto
            {
                IngresosTotalesUSD = ingresosTotalesUSD,
                CostosProveedorUSD = costosProveedorUSD,
                UtilidadBrutaUSD = utilidadBrutaUSD,
                GastosOperativosTotalesUSD = totalGastosUSD,
                UtilidadNetaUSD = utilidadNetaUSD,
                UtilidadNetaNIO = utilidadNetaUSD * config.TipoCambioNIO,
                DesgloseGastosUSD = desglose
            });
        }

        private (bool EsValido, List<int> TargetIds, ActionResult? ErrorResult) ValidarAlcanceSucursales(int? sucursalIdParam)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            if (userRole == "SUPER_ADMIN")
            {
                if (sucursalIdParam.HasValue)
                    return (true, new List<int> { sucursalIdParam.Value }, null);

                return (true, new List<int> { 1, 2, 3 }, null);
            }

            if (sucursalIdParam.HasValue && sucursalIdParam.Value != miSucursalId)
                return (false, new List<int>(), Forbid());

            return (true, new List<int> { miSucursalId }, null);
        }
    }
}