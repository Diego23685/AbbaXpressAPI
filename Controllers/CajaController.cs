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
    public class CajaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CajaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/caja/estado-actual
        [HttpGet("estado-actual")]
        public async Task<ActionResult<object>> GetEstadoActual([FromQuery] int? sucursalId)
        {
            int targetSucursalId = ObtenerSucursalContexto(sucursalId);

            var sesionAbierta = await _context.SesionesCaja
                .Include(s => s.UsuarioApertura)
                .Include(s => s.Denominaciones)
                .Include(s => s.Movimientos)
                .FirstOrDefaultAsync(s => s.SucursalId == targetSucursalId && s.Estado == "ABIERTA");

            if (sesionAbierta == null)
            {
                return Ok(new { abierta = false, mensaje = "La sucursal no tiene una caja abierta actualmente." });
            }

            // Calcular ingresos de facturas cobradas en efectivo dentro de esta sesión
            var proformasEfectivo = await _context.Proformas
                .Where(p => p.SesionCajaId == sesionAbierta.Id && p.Estado == "FACTURADO")
                .ToListAsync();

            decimal ingresosNio = proformasEfectivo.Where(p => p.MetodoPago == "EFECTIVO_NIO").Sum(p => p.TotalCobradoUSD * p.TipoCambioAplicado);
            decimal ingresosUsd = proformasEfectivo.Where(p => p.MetodoPago == "EFECTIVO_USD").Sum(p => p.TotalCobradoUSD);

            decimal egresosValesNio = sesionAbierta.Movimientos.Where(m => m.Moneda == "NIO" && m.TipoMovimiento == "EGRESO_VALE").Sum(m => m.Monto);
            decimal egresosValesUsd = sesionAbierta.Movimientos.Where(m => m.Moneda == "USD" && m.TipoMovimiento == "EGRESO_VALE").Sum(m => m.Monto);

            return Ok(new
            {
                abierta = true,
                sesionId = sesionAbierta.Id,
                sesionAbierta.FechaApertura,
                Cajero = sesionAbierta.UsuarioApertura?.Nombre,
                sesionAbierta.BultosFisicosApertura,
                sesionAbierta.MontoAperturaCordobas,
                sesionAbierta.MontoAperturaDolares,
                SaldoActualTeoricoNio = (sesionAbierta.MontoAperturaCordobas + ingresosNio) - egresosValesNio,
                SaldoActualTeoricoUsd = (sesionAbierta.MontoAperturaDolares + ingresosUsd) - egresosValesUsd,
                TotalValesEmitidos = sesionAbierta.Movimientos.Count,
                DenominacionesApertura = sesionAbierta.Denominaciones.Where(d => d.Momento == "APERTURA")
            });
        }

        // POST: api/caja/apertura
        [HttpPost("apertura")]
        public async Task<IActionResult> AperturarCaja([FromBody] AperturaCajaDto dto)
        {
            int targetSucursalId = ObtenerSucursalContexto(dto.SucursalId);
            int usuarioId = ObtenerUsuarioId();

            bool yaExisteAbierta = await _context.SesionesCaja
                .AnyAsync(s => s.SucursalId == targetSucursalId && s.Estado == "ABIERTA");

            if (yaExisteAbierta)
                return BadRequest(new { message = "Ya existe una sesión de caja abierta en esta sucursal." });

            decimal totalAperturaNio = dto.Denominaciones.Where(d => d.Moneda == "NIO").Sum(d => d.Denominacion * d.Cantidad);
            decimal totalAperturaUsd = dto.Denominaciones.Where(d => d.Moneda == "USD").Sum(d => d.Denominacion * d.Cantidad);

            var nuevaSesion = new SesionCaja
            {
                SucursalId = targetSucursalId,
                UsuarioAperturaId = usuarioId,
                FechaApertura = DateTime.UtcNow,
                BultosFisicosApertura = dto.BultosFisicosApertura,
                MontoAperturaCordobas = totalAperturaNio,
                MontoAperturaDolares = totalAperturaUsd,
                Estado = "ABIERTA"
            };

            foreach (var den in dto.Denominaciones)
            {
                if (den.Cantidad > 0)
                {
                    nuevaSesion.Denominaciones.Add(new DenominacionCaja
                    {
                        Momento = "APERTURA",
                        Moneda = den.Moneda.ToUpper(),
                        Denominacion = den.Denominacion,
                        Cantidad = den.Cantidad
                    });
                }
            }

            _context.SesionesCaja.Add(nuevaSesion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Caja aperturada exitosamente.", sesionId = nuevaSesion.Id });
        }

        // POST: api/caja/vales
        [HttpPost("vales")]
        public async Task<IActionResult> RegistrarValeEgreso([FromBody] ValeCajaDto dto)
        {
            int targetSucursalId = ObtenerSucursalContexto(null);
            int usuarioId = ObtenerUsuarioId();

            var sesion = await _context.SesionesCaja
                .FirstOrDefaultAsync(s => s.SucursalId == targetSucursalId && s.Estado == "ABIERTA");

            if (sesion == null)
                return BadRequest(new { message = "Debe aperturar caja antes de registrar salidas o vales de efectivo." });

            var vale = new MovimientoCaja
            {
                SesionCajaId = sesion.Id,
                UsuarioId = usuarioId,
                TipoMovimiento = "EGRESO_VALE",
                Concepto = dto.Concepto.Trim(),
                Beneficiario = dto.Beneficiario.Trim(),
                Monto = dto.Monto,
                Moneda = dto.Moneda.ToUpper(),
                FechaRegistro = DateTime.UtcNow
            };

            _context.MovimientosCaja.Add(vale);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vale de caja registrado correctamente.", valeId = vale.Id });
        }

        // POST: api/caja/cierre
        [HttpPost("cierre")]
        public async Task<IActionResult> CerrarCaja([FromBody] CierreCajaDto dto)
        {
            int targetSucursalId = ObtenerSucursalContexto(null);
            int usuarioId = ObtenerUsuarioId();

            var sesion = await _context.SesionesCaja
                .Include(s => s.Denominaciones)
                .FirstOrDefaultAsync(s => s.SucursalId == targetSucursalId && s.Estado == "ABIERTA");

            if (sesion == null)
                return BadRequest(new { message = "No hay ninguna caja abierta para realizar el corte." });

            decimal totalCierreNio = dto.Denominaciones.Where(d => d.Moneda == "NIO").Sum(d => d.Denominacion * d.Cantidad);
            decimal totalCierreUsd = dto.Denominaciones.Where(d => d.Moneda == "USD").Sum(d => d.Denominacion * d.Cantidad);

            sesion.UsuarioCierreId = usuarioId;
            sesion.FechaCierre = DateTime.UtcNow;
            sesion.BultosFisicosCierre = dto.BultosFisicosCierre;
            sesion.MontoCierreCordobas = totalCierreNio;
            sesion.MontoCierreDolares = totalCierreUsd;
            sesion.Estado = "CERRADA";

            foreach (var den in dto.Denominaciones)
            {
                if (den.Cantidad > 0)
                {
                    sesion.Denominaciones.Add(new DenominacionCaja
                    {
                        Momento = "CIERRE",
                        Moneda = den.Moneda.ToUpper(),
                        Denominacion = den.Denominacion,
                        Cantidad = den.Cantidad
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Corte de caja finalizado exitosamente." });
        }

        private int ObtenerSucursalContexto(int? paramSucursalId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            if (userRole == "SUPER_ADMIN" && paramSucursalId.HasValue)
                return paramSucursalId.Value;

            return miSucursalId;
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int uId) ? uId : 1;
        }
    }
}