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
    public class ProformasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProformasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/proformas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProformaResponseDto>>> GetProformas(
            [FromQuery] string? estado,
            [FromQuery] int? sucursalId,
            [FromQuery] int? sucursalOrigenId,
            [FromQuery] int? sucursalDestinoId,
            [FromQuery] string? busqueda)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            var query = _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.SucursalOrigen)
                .Include(p => p.SucursalDestino)
                .Include(p => p.Paquetes)
                .AsQueryable();

            if (userRole != "SUPER_ADMIN")
            {
                query = query.Where(p => p.SucursalOrigenId == miSucursalId || p.SucursalDestinoId == miSucursalId);
            }
            else
            {
                if (sucursalOrigenId.HasValue)
                    query = query.Where(p => p.SucursalOrigenId == sucursalOrigenId.Value);
                else if (sucursalDestinoId.HasValue)
                    query = query.Where(p => p.SucursalDestinoId == sucursalDestinoId.Value);
                else
                    query = query.Where(p => targetIds.Contains(p.SucursalOrigenId) || targetIds.Contains(p.SucursalDestinoId));
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                var estadosLista = estado.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                query = query.Where(p => estadosLista.Contains(p.Estado));
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.ToLower().Trim();
                query = query.Where(p => 
                    p.NumeroProforma.ToLower().Contains(term) ||
                    (p.Cliente != null && p.Cliente.Nombre.ToLower().Contains(term)) ||
                    p.Paquetes.Any(pkg => pkg.Tracking.ToLower().Contains(term)));
            }

            var lista = await query
                .OrderByDescending(p => p.Id)
                .Select(p => new ProformaResponseDto
                {
                    Id = p.Id,
                    NumeroProforma = p.NumeroProforma,
                    ClienteId = p.ClienteId,
                    ClienteNombre = p.Cliente != null ? p.Cliente.Nombre : "Sin Asignar",
                    ClienteTelefono = p.Cliente != null ? $"{p.Cliente.CodigoPais} {p.Cliente.Telefono}" : "",
                    SucursalOrigen = p.SucursalOrigen != null ? p.SucursalOrigen.Nombre : "",
                    SucursalDestino = p.SucursalDestino != null ? p.SucursalDestino.Nombre : "",
                    Estado = p.Estado,
                    MetodoPago = p.MetodoPago,
                    TotalLbs = p.TotalLbs,
                    CargoDeliveryUSD = p.CargoDeliveryUSD,
                    DescuentoUSD = p.DescuentoUSD,
                    TotalCobradoUSD = p.TotalCobradoUSD,
                    TotalCobradoNIO = p.TotalCobradoUSD * p.TipoCambioAplicado,
                    TotalCostoProveedorUSD = p.TotalCostoProveedorUSD,
                    UtilidadBrutaUSD = p.TotalCobradoUSD - p.TotalCostoProveedorUSD,
                    TipoCambioAplicado = p.TipoCambioAplicado,
                    FechaRegistro = p.FechaRegistro,
                    FechaFacturacion = p.FechaFacturacion,
                    Paquetes = p.Paquetes.Select(pkg => new PaqueteResponseDto
                    {
                        Id = pkg.Id,
                        Tracking = pkg.Tracking,
                        Label = pkg.Label,
                        PesoLbs = pkg.PesoLbs,
                        ViaEnvio = pkg.ViaEnvio,
                        Categoria = pkg.Categoria,
                        TarifaAplicada = pkg.TarifaAplicada,
                        CostoProveedor = pkg.CostoProveedor,
                        SubtotalUSD = pkg.SubtotalUSD
                    }).ToList()
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/proformas/transferencias-pendientes
        [HttpGet("transferencias-pendientes")]
        public async Task<ActionResult<IEnumerable<object>>> GetTransferenciasPendientes()
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var proformas = await _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.SucursalOrigen)
                .Include(p => p.SucursalDestino)
                .Include(p => p.Paquetes)
                .Where(p => p.SucursalDestinoId == miSucursalId && p.SucursalOrigenId != miSucursalId && p.Estado == "PENDIENTE_PAGO")
                .OrderByDescending(p => p.FechaRegistro)
                .Select(p => new
                {
                    p.Id,
                    p.NumeroProforma,
                    Cliente = p.Cliente != null ? p.Cliente.Nombre : "Sin Asignar",
                    Telefono = p.Cliente != null ? $"{p.Cliente.CodigoPais} {p.Cliente.Telefono}" : "",
                    p.Estado,
                    p.TotalLbs,
                    TotalCobradoUSD = p.TotalCobradoUSD,
                    p.FechaRegistro,
                    Paquetes = p.Paquetes.Select(pkg => new
                    {
                        pkg.Id,
                        pkg.Tracking,
                        pkg.Label,
                        pkg.PesoLbs,
                        pkg.ViaEnvio,
                        pkg.Categoria
                    })
                })
                .ToListAsync();

            return Ok(proformas);
        }

        // PUT: api/proformas/5/editar-paquetes
        [HttpPut("{id}/editar-paquetes")]
        public async Task<IActionResult> EditarPaquetes(int id, [FromBody] List<PaqueteUpdateDto> paquetesDto)
        {
            var proforma = await _context.Proformas
                .Include(p => p.Paquetes)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proforma == null)
                return NotFound(new { message = "Proforma no encontrada." });

            if (proforma.Estado == "FACTURADO")
                return BadRequest(new { message = "No se pueden modificar paquetes de una proforma ya facturada y cobrada." });

            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == proforma.SucursalOrigenId)
                        ?? new ConfiguracionSucursal { TipoCambioNIO = 36.6243m, CostoProveedorAereo = 3.80m, CostoProveedorMaritimo = 1.50m };

            decimal totalLbs = 0;
            decimal subtotalCargaUSD = 0;
            decimal totalCostoProveedorUSD = 0;

            foreach (var dto in paquetesDto)
            {
                var paquete = proforma.Paquetes.FirstOrDefault(p => p.Id == dto.Id);
                if (paquete == null) continue;

                decimal subtotal = (dto.Categoria.ToUpper() == "CELULAR" || dto.Categoria.ToUpper() == "PALLET")
                    ? dto.TarifaAplicada
                    : dto.PesoLbs * dto.TarifaAplicada;

                decimal costoLibra = dto.ViaEnvio.ToUpper() == "MARITIMO" ? config.CostoProveedorMaritimo : config.CostoProveedorAereo;
                decimal costoProveedor = (dto.Categoria.ToUpper() == "CELULAR") ? 15.00m : dto.PesoLbs * costoLibra;

                paquete.Tracking = dto.Tracking.Trim().ToUpper();
                paquete.PesoLbs = dto.PesoLbs;
                paquete.ViaEnvio = dto.ViaEnvio.ToUpper();
                paquete.Categoria = dto.Categoria.ToUpper();
                paquete.TarifaAplicada = dto.TarifaAplicada;
                paquete.SubtotalUSD = subtotal;
                paquete.CostoProveedor = costoProveedor;

                totalLbs += dto.PesoLbs;
                subtotalCargaUSD += subtotal;
                totalCostoProveedorUSD += costoProveedor;
            }

            proforma.TotalLbs = totalLbs;
            proforma.TotalCobradoUSD = Math.Max(0, (subtotalCargaUSD + proforma.CargoDeliveryUSD) - proforma.DescuentoUSD);
            proforma.TotalCostoProveedorUSD = totalCostoProveedorUSD;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = proforma.SucursalOrigenId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "PROFORMAS",
                Descripcion = $"El usuario {nombreUsuario} corrigió peso/tarifas en los bultos de la proforma #{proforma.NumeroProforma}.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Paquetes actualizados correctamente." });
        }

        // PUT: api/proformas/despachar-lote
        [HttpPut("despachar-lote")]
        public async Task<IActionResult> DespacharLote([FromBody] List<int> proformaIds)
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            if (proformaIds == null || !proformaIds.Any())
                return BadRequest(new { message = "Debe enviar al menos una proforma para despachar." });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";
            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;

            var proformas = await _context.Proformas
                .Where(p => proformaIds.Contains(p.Id) && p.Estado == "PENDIENTE_PAGO" && p.SucursalOrigenId == miSucursalId)
                .ToListAsync();

            if (!proformas.Any())
                return BadRequest(new { message = "Las proformas seleccionadas ya fueron despachadas o no pertenecen a su sede." });

            foreach (var p in proformas)
            {
                p.Estado = "EN_TRANSITO";
            }

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "PROFORMAS",
                Descripcion = $"El usuario {nombreUsuario} despachó {proformas.Count} carga(s) en tránsito.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Se despacharon {proformas.Count} cargas en ruta." });
        }

        // PUT: api/proformas/recibir-lote
        [HttpPut("recibir-lote")]
        public async Task<IActionResult> RecibirLote([FromBody] List<int> proformaIds)
        {
            if (proformaIds == null || !proformaIds.Any())
                return BadRequest(new { message = "Debe enviar al menos una proforma." });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";
            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var proformas = await _context.Proformas
                .Where(p => proformaIds.Contains(p.Id) && p.SucursalDestinoId == miSucursalId && p.Estado == "EN_TRANSITO")
                .ToListAsync();

            if (!proformas.Any())
                return BadRequest(new { message = "No hay cargas en tránsito pendientes de recibir con los IDs enviados." });

            foreach (var p in proformas)
            {
                p.Estado = "RECIBIDO_BODEGA_LOCAL"; 
            }

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "PROFORMAS",
                Descripcion = $"El usuario {nombreUsuario} confirmó la recepción de {proformas.Count} carga(s) (Proformas: {string.Join(", ", proformas.Select(x => x.NumeroProforma))}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Se confirmaron {proformas.Count} cargas recibidas en bodega." });
        }

        // GET: api/proformas/{id}/whatsapp-template
        [HttpGet("{id}/whatsapp-template")]
        public async Task<ActionResult<object>> GetWhatsAppTemplate(int id)
        {
            var proforma = await _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.Paquetes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proforma == null)
                return NotFound(new { message = "Proforma no encontrada." });

            var cliente = proforma.Cliente;
            string telefonoCompleto = $"{cliente?.CodigoPais ?? "+505"}{cliente?.Telefono ?? ""}".Replace("+", "").Replace(" ", "");

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"*ABBA XPRESS - NOTIFICACIÓN DE CARGA*");
            sb.AppendLine($"Hola *{cliente?.Nombre}*, tus paquetes ya están listos para retiro/entrega.");
            sb.AppendLine();
            sb.AppendLine($"*Proforma:* #{proforma.NumeroProforma}");
            sb.AppendLine($"*Peso Total:* {proforma.TotalLbs:F2} lbs");
            sb.AppendLine($"*Cant. Paquetes:* {proforma.Paquetes.Count}");
            sb.AppendLine();

            foreach (var pkg in proforma.Paquetes)
            {
                string label = string.IsNullOrEmpty(pkg.Label) ? "Paquete" : pkg.Label;
                sb.AppendLine($"• {label} ({pkg.Tracking}): {pkg.PesoLbs:F2} lb - ${pkg.SubtotalUSD:F2} USD");
            }

            if (proforma.CargoDeliveryUSD > 0)
                sb.AppendLine($"*Delivery:* ${proforma.CargoDeliveryUSD:F2} USD");

            decimal totalNio = proforma.TotalCobradoUSD * proforma.TipoCambioAplicado;

            sb.AppendLine();
            sb.AppendLine($"*TOTAL A PAGAR:*");
            sb.AppendLine($"*${proforma.TotalCobradoUSD:F2} USD*  |  *C$ {totalNio:F2} NIO*");

            return Ok(new
            {
                telefono = telefonoCompleto,
                mensaje = sb.ToString(),
                enlaceDirecto = $"https://api.whatsapp.com/send?phone={telefonoCompleto}&text={Uri.EscapeDataString(sb.ToString())}"
            });
        }

        // GET: api/proformas/resumen-cobros
        [HttpGet("resumen-cobros")]
        public async Task<ActionResult<object>> GetResumenCobros([FromQuery] int? sucursalId)
        {
            var (esValido, targetIds, errorResult) = ValidarAlcanceSucursales(sucursalId);
            if (!esValido) return errorResult!;

            var query = _context.Proformas
                .Where(p => targetIds.Contains(p.SucursalOrigenId))
                .AsQueryable();

            var pendientes = await query.Where(p => p.Estado == "PENDIENTE_PAGO" || p.Estado == "EN_TRANSITO" || p.Estado == "RECIBIDO_BODEGA_LOCAL").ToListAsync();
            var facturadas = await query.Where(p => p.Estado == "FACTURADO").ToListAsync();

            decimal totalPorCobrarUSD = pendientes.Sum(p => p.TotalCobradoUSD);
            decimal totalFacturadoUSD = facturadas.Sum(p => p.TotalCobradoUSD);
            decimal totalLibrasPendientes = pendientes.Sum(p => p.TotalLbs);

            return Ok(new
            {
                totalPorCobrarUSD,
                totalFacturadoUSD,
                totalLibrasPendientes,
                cantidadPendientes = pendientes.Count,
                cantidadFacturadas = facturadas.Count
            });
        }

        // POST: api/proformas (Recepción de Carga)
        [HttpPost]
        public async Task<ActionResult<object>> CreateProforma([FromBody] RecepcionCargaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                return BadRequest(new { message = "El cliente especificado no existe." });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalIdClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int sucursalOrigenId = int.TryParse(sucursalIdClaim, out int sId) ? sId : 1;

            // Detección genérica de sucursal destino
            int sucursalDestinoId = dto.SucursalDestinoId.HasValue && dto.SucursalDestinoId.Value > 0
                ? dto.SucursalDestinoId.Value
                : sucursalOrigenId;

            var sedeDestino = await _context.Sucursales.FindAsync(sucursalDestinoId);
            bool esDestinoSubagencia = (sedeDestino != null && sedeDestino.TipoSucursal == "FRANQUICIA_B2B")
                                    || cliente.TipoCliente == "SUBAGENCIA" 
                                    || cliente.TipoCliente == "CLIENTE_MAYORISTA";

            int configSucursalId = sucursalOrigenId == 2 ? 1 : sucursalOrigenId;
            var config = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == configSucursalId)
                        ?? new ConfiguracionSucursal { SucursalId = sucursalOrigenId };

            var proveedores = await _context.Proveedores.Where(p => p.Activo).ToDictionaryAsync(p => p.Id);

            var totalProformas = await _context.Proformas.CountAsync();
            string numeroCorrelativo = $"ABBA-{1001 + totalProformas}";

            decimal totalLibras = 0;
            decimal subtotalCargaUSD = 0;
            decimal totalCostoProveedorUSD = 0;

            var nuevosPaquetes = new List<Paquete>();

            foreach (var item in dto.Paquetes)
            {
                decimal costoProveedorLibra = item.ViaEnvio == "MARITIMO" ? config.CostoProveedorMaritimo : config.CostoProveedorAereo;
                if (item.ProveedorId.HasValue && proveedores.TryGetValue(item.ProveedorId.Value, out var prov))
                {
                    costoProveedorLibra = item.ViaEnvio == "MARITIMO" ? prov.CostoLibraMaritimo : prov.CostoLibraAereo;
                }

                decimal costoProveedorTotal = item.PesoLbs * costoProveedorLibra;
                decimal tarifa = 0;
                decimal subtotalItem = 0;

                switch (item.Categoria.ToUpper())
                {
                    case "CELULAR":
                        tarifa = item.TarifaManual ?? cliente.TarifaCelularFija ?? config.TarifaCelularFija;
                        costoProveedorTotal = 15.00m;
                        subtotalItem = tarifa;
                        break;

                    case "SMART_TV":
                        tarifa = item.TarifaManual ?? (item.ViaEnvio == "MARITIMO" 
                            ? (cliente.TarifaTvMaritimo ?? config.TarifaTvMaritimo) 
                            : (cliente.TarifaTvAereo ?? config.TarifaTvAereo));
                        subtotalItem = item.PesoLbs * tarifa;
                        break;

                    case "PALLET":
                        tarifa = item.TarifaManual ?? 0.00m;
                        subtotalItem = tarifa;
                        break;

                    default:
                        tarifa = item.TarifaManual ?? (item.ViaEnvio == "MARITIMO" ? cliente.TarifaMaritimo : cliente.TarifaAereo);
                        subtotalItem = item.PesoLbs * tarifa;
                        break;
                }

                totalLibras += item.PesoLbs;
                subtotalCargaUSD += subtotalItem;
                totalCostoProveedorUSD += costoProveedorTotal;

                nuevosPaquetes.Add(new Paquete
                {
                    Tracking = item.Tracking.Trim().ToUpper(),
                    PesoLbs = item.PesoLbs,
                    ViaEnvio = item.ViaEnvio.ToUpper(),
                    Categoria = item.Categoria.ToUpper(),
                    TarifaAplicada = tarifa,
                    ProveedorId = item.ProveedorId,
                    CostoProveedor = costoProveedorTotal,
                    SubtotalUSD = subtotalItem
                });
            }

            var proforma = new Proforma
            {
                NumeroProforma = numeroCorrelativo,
                ClienteId = dto.ClienteId,
                SucursalOrigenId = sucursalOrigenId,
                SucursalDestinoId = sucursalDestinoId,
                UsuarioCreacionId = usuarioId,
                Estado = "PENDIENTE_PAGO",
                MetodoPago = "CREDITO",
                TotalLbs = totalLibras,
                CargoDeliveryUSD = 0.00m,
                DescuentoUSD = 0.00m,
                TotalCobradoUSD = subtotalCargaUSD,
                TotalCostoProveedorUSD = totalCostoProveedorUSD,
                TipoCambioAplicado = config.TipoCambioNIO,
                EsCargaInterSucursal = esDestinoSubagencia,
                FechaRegistro = DateTime.UtcNow,
                FechaFacturacion = null,
                Paquetes = nuevosPaquetes
            };

            _context.Proformas.Add(proforma);

            if (esDestinoSubagencia && sucursalDestinoId != sucursalOrigenId)
            {
                _context.NotificacionesSucursales.Add(new NotificacionSucursal
                {
                    SucursalDestinoId = sucursalDestinoId,
                    Proforma = proforma,
                    Mensaje = $"Nueva carga registrada hacia su sede (#{numeroCorrelativo} - {totalLibras:F2} lbs).",
                    Leida = false,
                    FechaCreacion = DateTime.UtcNow
                });
            }

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = sucursalOrigenId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "RECEPCION",
                Descripcion = $"Recepción de {nuevosPaquetes.Count} bultos (#{numeroCorrelativo}) para '{cliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carga recepcionada correctamente.",
                proformaId = proforma.Id,
                numeroProforma = proforma.NumeroProforma,
                totalLibras = proforma.TotalLbs,
                totalCobradoUSD = proforma.TotalCobradoUSD
            });
        }

        // PUT: api/proformas/5/liquidar
        [HttpPut("{id}/liquidar")]
        public async Task<IActionResult> LiquidarProforma(int id, [FromBody] LiquidarProformaDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sucursalClaim = User.FindFirstValue("SucursalId");
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var proforma = await _context.Proformas
                .Include(p => p.Cliente)
                .Include(p => p.Paquetes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proforma == null)
                return NotFound(new { message = "Proforma no encontrada." });

            var sesionCaja = await _context.SesionesCaja
                .FirstOrDefaultAsync(s => s.SucursalId == miSucursalId && s.Estado == "ABIERTA");

            if (sesionCaja == null)
                return BadRequest(new { message = "Operación denegada: Debe aperturar caja antes de cobrar y facturar paquetes." });

            decimal totalFlete = proforma.Paquetes.Sum(p => p.SubtotalUSD);
            decimal totalFinalUSD = Math.Max(0, (totalFlete + dto.CargoDeliveryUSD) - dto.DescuentoUSD);

            proforma.MetodoPago = dto.MetodoPago.ToUpper();
            proforma.CargoDeliveryUSD = dto.CargoDeliveryUSD;
            proforma.DescuentoUSD = dto.DescuentoUSD;
            proforma.TotalCobradoUSD = totalFinalUSD;
            proforma.Estado = "FACTURADO";
            proforma.FechaFacturacion = DateTime.UtcNow;
            proforma.SesionCajaId = sesionCaja.Id;

            if (dto.MetodoPago.ToUpper() == "POS")
            {
                proforma.PorcentajeComisionPOS = dto.PorcentajeComisionPOS;
                proforma.MontoComisionPOS = totalFinalUSD * (dto.PorcentajeComisionPOS / 100m);
            }

            Dictionary<decimal, int>? desgloseVuelto = null;
            if (dto.MetodoPago.StartsWith("EFECTIVO"))
            {
                decimal totalPagarEnMoneda = dto.MonedaPagoEfectivo == "NIO" 
                    ? totalFinalUSD * proforma.TipoCambioAplicado 
                    : totalFinalUSD;

                if (dto.MontoRecibidoEfectivo < totalPagarEnMoneda)
                    return BadRequest(new { message = $"Monto recibido ({dto.MontoRecibidoEfectivo}) es menor al total a cobrar ({totalPagarEnMoneda:F2})." });

                decimal vuelto = dto.MontoRecibidoEfectivo - totalPagarEnMoneda;
                proforma.MontoRecibidoEfectivo = dto.MontoRecibidoEfectivo;
                proforma.MontoVueltoEntregado = vuelto;
                desgloseVuelto = CalcularDesgloseVuelto(vuelto, dto.MonedaPagoEfectivo);
            }

            // Gasto operativo automático en la subagencia receptora
            var sedeDestino = await _context.Sucursales.FindAsync(proforma.SucursalDestinoId);
            if (sedeDestino != null && sedeDestino.TipoSucursal == "FRANQUICIA_B2B" && proforma.SucursalOrigenId != proforma.SucursalDestinoId)
            {
                var gastoSubagencia = new GastoOperativo
                {
                    SucursalId = proforma.SucursalDestinoId,
                    UsuarioId = usuarioId,
                    Categoria = "FLETE_MATRIZ",
                    Descripcion = $"Flete recepcionado s/ Factura #{proforma.NumeroProforma} ({proforma.Cliente?.Nombre})",
                    MontoUSD = totalFinalUSD,
                    TipoCambioAplicado = proforma.TipoCambioAplicado,
                    MetodoPago = proforma.MetodoPago,
                    NumeroComprobante = proforma.NumeroProforma,
                    FechaGasto = DateTime.UtcNow
                };

                _context.GastosOperativos.Add(gastoSubagencia);
                await _context.SaveChangesAsync();
                proforma.GastoGeneradoLeonId = gastoSubagencia.Id;
            }

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "COBRO",
                Modulo = "FACTURACION",
                Descripcion = $"Cobro exitoso de proforma #{proforma.NumeroProforma} por ${totalFinalUSD:F2} USD ({dto.MetodoPago}).",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Factura #{proforma.NumeroProforma} cobrada exitosamente.",
                totalCobradoUSD = totalFinalUSD,
                vuelto = proforma.MontoVueltoEntregado,
                desgloseBilletesSugerido = desgloseVuelto
            });
        }

        private static Dictionary<decimal, int> CalcularDesgloseVuelto(decimal vueltoTotal, string moneda)
        {
            var desglose = new Dictionary<decimal, int>();
            if (vueltoTotal <= 0) return desglose;

            decimal[] denominaciones = moneda == "NIO" 
                ? new decimal[] { 1000m, 500m, 200m, 100m, 50m, 20m, 10m, 5m, 1m }
                : new decimal[] { 100m, 50m, 20m, 10m, 5m, 1m };

            decimal restante = Math.Round(vueltoTotal, 2);

            foreach (var d in denominaciones)
            {
                if (restante >= d)
                {
                    int cant = (int)(restante / d);
                    desglose.Add(d, cant);
                    restante %= d;
                }
            }

            return desglose;
        }

        // PUT: api/proformas/5/procesar-recepcion-subagencia
        [HttpPut("{id}/procesar-recepcion-subagencia")]
        public async Task<IActionResult> ProcesarRecepcionSubagencia(int id, [FromBody] RecepcionLeonDto dto)
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";
            int usuarioId = int.TryParse(userIdClaim, out int uId) ? uId : 1;

            var proformaOriginal = await _context.Proformas
                .Include(p => p.Paquetes)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id && p.SucursalDestinoId == miSucursalId);

            if (proformaOriginal == null)
                return NotFound(new { message = "Proforma no encontrada o no pertenece a esta sucursal de destino." });

            if (proformaOriginal.Estado == "PROCESADO_ASIGNADO")
                return BadRequest(new { message = $"Esta remisión (#{proformaOriginal.NumeroProforma}) ya fue asignada previamente a un cliente local." });

            var clienteLocal = await _context.Clientes.FindAsync(dto.ClienteId);
            if (clienteLocal == null)
                return BadRequest(new { message = "El cliente local seleccionado no existe." });

            var configLocal = await _context.Configuraciones.FirstOrDefaultAsync(c => c.SucursalId == miSucursalId)
                            ?? new ConfiguracionSucursal { SucursalId = miSucursalId };

            // 1. Marcar la remisión como completada y bloqueada
            proformaOriginal.Estado = "PROCESADO_ASIGNADO";

            // 2. Crear proforma final local
            var totalProformas = await _context.Proformas.CountAsync();
            string correlativoLocal = $"ABBA-{1001 + totalProformas}";

            decimal totalLibras = 0;
            decimal subtotalCargaUSD = 0;
            decimal totalCostoProveedorUSD = 0;

            var nuevosPaquetes = new List<Paquete>();

            foreach (var itemOriginal in proformaOriginal.Paquetes)
            {
                var pkgDto = dto.Paquetes.FirstOrDefault(p => p.Id == itemOriginal.Id);
                decimal peso = pkgDto != null && pkgDto.PesoLbs > 0 ? pkgDto.PesoLbs : itemOriginal.PesoLbs;
                decimal tarifa = pkgDto != null && pkgDto.TarifaAplicada > 0 ? pkgDto.TarifaAplicada : itemOriginal.TarifaAplicada;

                decimal subtotal = (itemOriginal.Categoria == "CELULAR" || itemOriginal.Categoria == "PALLET")
                    ? tarifa
                    : peso * tarifa;

                totalLibras += peso;
                subtotalCargaUSD += subtotal;
                totalCostoProveedorUSD += itemOriginal.TarifaAplicada;

                nuevosPaquetes.Add(new Paquete
                {
                    Tracking = itemOriginal.Tracking,
                    Label = itemOriginal.Label,
                    PesoLbs = peso,
                    ViaEnvio = itemOriginal.ViaEnvio,
                    Categoria = itemOriginal.Categoria,
                    TarifaAplicada = tarifa,
                    CostoProveedor = itemOriginal.TarifaAplicada,
                    SubtotalUSD = subtotal
                });
            }

            decimal totalFinalCobradoUSD = Math.Max(0, (subtotalCargaUSD + dto.CargoDeliveryUSD) - dto.DescuentoUSD);
            string estadoInicial = dto.MetodoPago == "CREDITO" ? "PENDIENTE_PAGO" : "FACTURADO";
            DateTime? fechaFacturacion = dto.MetodoPago == "CREDITO" ? null : DateTime.UtcNow;

            var nuevaProformaLocal = new Proforma
            {
                NumeroProforma = correlativoLocal,
                ClienteId = clienteLocal.Id,
                SucursalOrigenId = miSucursalId,
                SucursalDestinoId = miSucursalId,
                UsuarioCreacionId = usuarioId,
                Estado = estadoInicial,
                MetodoPago = dto.MetodoPago ?? "CREDITO",
                TotalLbs = totalLibras,
                CargoDeliveryUSD = dto.CargoDeliveryUSD,
                DescuentoUSD = dto.DescuentoUSD,
                TotalCobradoUSD = totalFinalCobradoUSD,
                TotalCostoProveedorUSD = totalCostoProveedorUSD,
                TipoCambioAplicado = configLocal.TipoCambioNIO,
                EsCargaInterSucursal = false,
                FechaRegistro = DateTime.UtcNow,
                FechaFacturacion = fechaFacturacion,
                Paquetes = nuevosPaquetes
            };

            _context.Proformas.Add(nuevaProformaLocal);

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = miSucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "PROFORMAS",
                Descripcion = $"Remisión #{proformaOriginal.NumeroProforma} asignada a '{clienteLocal.Nombre}'. Se generó factura local #{correlativoLocal}.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Carga asignada correctamente. Se generó la proforma local #{correlativoLocal} para {clienteLocal.Nombre}.",
                proformaId = nuevaProformaLocal.Id,
                numero = nuevaProformaLocal.NumeroProforma
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
            {
                return (false, new List<int>(), Forbid());
            }

            return (true, new List<int> { miSucursalId }, null);
        }
    }
}