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
    public class ExportacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExportacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvioExportacionResponseDto>>> GetEnvios([FromQuery] string? busqueda, [FromQuery] int? sucursalId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.EnviosExportacion
                .Include(e => e.Items)
                .AsQueryable();

            // Aislamiento estricto por sede
            if (userRole == "SUPER_ADMIN")
            {
                if (sucursalId.HasValue)
                {
                    query = query.Where(e => e.SucursalOrigenId == sucursalId.Value);
                }
            }
            else
            {
                // Administrador local: solo ve los envíos de su propia sucursal
                query = query.Where(e => e.SucursalOrigenId == miSucursalId);
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.ToLower().Trim();
                query = query.Where(e => 
                    e.CodigoEnvio.ToLower().Contains(term) ||
                    e.RemitenteNombre.ToLower().Contains(term) ||
                    e.DestinatarioNombre.ToLower().Contains(term) ||
                    (e.TrackingFedEx != null && e.TrackingFedEx.ToLower().Contains(term)));
            }

            var list = await query
                .OrderByDescending(e => e.Id)
                .Select(e => new EnvioExportacionResponseDto
                {
                    Id = e.Id,
                    CodigoEnvio = e.CodigoEnvio,
                    TrackingFedEx = e.TrackingFedEx,
                    RemitenteNombre = e.RemitenteNombre,
                    RemitenteTelefono = e.RemitenteTelefono,
                    DestinatarioNombre = e.DestinatarioNombre,
                    DestinatarioEstado = e.DestinatarioEstado,
                    DestinatarioCiudad = e.DestinatarioCiudad,
                    PesoTotalLbs = e.PesoTotalLbs,
                    TarifaBaseUSD = e.TarifaBaseUSD,
                    RecargoEstadoUSD = e.RecargoEstadoUSD,
                    TotalCobradoUSD = e.TotalCobradoUSD,
                    TotalCobradoNIO = e.TotalCobradoUSD * e.TipoCambioAplicado,
                    EstadoOperativo = e.EstadoOperativo,
                    FechaRegistro = e.FechaRegistro,
                    Items = e.Items.Select(i => new ItemExportacionCreateDto
                    {
                        DescripcionES = i.DescripcionES,
                        DescripcionEN = i.DescripcionEN,
                        Cantidad = i.Cantidad,
                        PesoLbs = i.PesoLbs,
                        ValorDeclaradoUSD = i.ValorDeclaradoUSD
                    }).ToList()
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<EnvioExportacionResponseDto>> CreateEnvio([FromBody] EnvioExportacionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalIdClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";
            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int sucursalId = int.TryParse(sucursalIdClaim, out int sId) ? sId : 1;

            decimal tarifaBase = 0;
            decimal peso = dto.PesoTotalLbs;

            if (dto.TarifaManualUSD.HasValue && dto.TarifaManualUSD.Value > 0)
            {
                tarifaBase = dto.TarifaManualUSD.Value;
            }
            else
            {
                if (peso <= 10) tarifaBase = 190.00m;
                else if (peso <= 20) tarifaBase = 250.00m;
                else if (peso <= 30) tarifaBase = 350.00m;
                else tarifaBase = 450.00m;
            }

            decimal recargoEstado = (dto.DestinatarioEstado.Trim().ToUpper() == "FL" || 
                                     dto.DestinatarioEstado.Trim().ToUpper() == "FLORIDA") ? 0.00m : 10.00m;

            decimal totalFinalUSD = tarifaBase + recargoEstado;

            var totalEnvios = await _context.EnviosExportacion.CountAsync();
            string codigoGenerado = $"EXP-{2001 + totalEnvios}";

            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == sucursalId)
                        ?? new ConfiguracionSucursal { SucursalId = sucursalId };

            var nuevoEnvio = new EnvioExportacion
            {
                CodigoEnvio = codigoGenerado,
                TrackingFedEx = dto.TrackingFedEx,
                SucursalOrigenId = sucursalId,
                UsuarioId = usuarioId,
                RemitenteNombre = dto.RemitenteNombre,
                RemitenteTelefono = dto.RemitenteTelefono,
                RemitenteDireccion = dto.RemitenteDireccion,
                DestinatarioNombre = dto.DestinatarioNombre,
                DestinatarioTelefono = dto.DestinatarioTelefono,
                DestinatarioEstado = dto.DestinatarioEstado.ToUpper(),
                DestinatarioCiudad = dto.DestinatarioCiudad,
                DestinatarioZipCode = dto.DestinatarioZipCode,
                DestinatarioDireccion = dto.DestinatarioDireccion,
                PesoTotalLbs = peso,
                TarifaBaseUSD = tarifaBase,
                RecargoEstadoUSD = recargoEstado,
                TotalCobradoUSD = totalFinalUSD,
                TipoCambioAplicado = config.TipoCambioNIO,
                EstadoOperativo = "RECEPCIONADO_NICARAGUA",
                FechaRegistro = DateTime.UtcNow,
                Items = dto.Items.Select(i => new ItemExportacion
                {
                    DescripcionES = i.DescripcionES,
                    DescripcionEN = i.DescripcionEN,
                    Cantidad = i.Cantidad,
                    PesoLbs = i.PesoLbs,
                    ValorDeclaradoUSD = i.ValorDeclaradoUSD
                }).ToList()
            };

            _context.EnviosExportacion.Add(nuevoEnvio);

            // Registro de auditoría por creación de exportación
            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = sucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "EXPORTACION",
                Descripcion = $"El usuario {nombreUsuario} registró el envío de exportación #{codigoGenerado} con destino a {nuevoEnvio.DestinatarioCiudad}, {nuevoEnvio.DestinatarioEstado} (${totalFinalUSD:F2} USD).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Envío de exportación creado con éxito", codigo = nuevoEnvio.CodigoEnvio });
        }
    }
}