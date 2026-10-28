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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientes([FromQuery] string? busqueda, [FromQuery] int? sucursalId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.Clientes.Where(c => c.Activo).AsQueryable();

            var idsPropias = await _context.Sucursales
                .Where(s => s.Activa && s.TipoSucursal == "PROPIA")
                .Select(s => s.Id)
                .ToListAsync();

            if (userRole == "SUPER_ADMIN")
            {
                if (sucursalId.HasValue)
                {
                    if (!idsPropias.Contains(sucursalId.Value))
                        return StatusCode(StatusCodes.Status403Forbidden, new { message = "Acceso denegado a clientes de franquicias B2B." });

                    query = query.Where(c => c.SucursalId == sucursalId.Value);
                }
                else
                {
                    query = query.Where(c => idsPropias.Contains(c.SucursalId));
                }
            }
            else
            {
                var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
                if (miSucursal?.TipoSucursal == "PROPIA")
                {
                    query = query.Where(c => idsPropias.Contains(c.SucursalId));
                }
                else
                {
                    query = query.Where(c => c.SucursalId == miSucursalId);
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.ToLower().Trim();
                query = query.Where(c => 
                    c.Nombre.ToLower().Contains(term) || 
                    c.Telefono.Contains(term) || 
                    (c.Email != null && c.Email.ToLower().Contains(term)));
            }

            var lista = await query
                .OrderByDescending(c => c.Id)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    CodigoPais = c.CodigoPais,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    TarifaAereo = c.TarifaAereo,
                    TarifaMaritimo = c.TarifaMaritimo,
                    Direccion = c.Direccion,
                    TipoCliente = c.TipoCliente,
                    Activo = c.Activo,
                    FechaRegistro = c.FechaRegistro
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/clientes/5/historial-360
        [HttpGet("{id}/historial-360")]
        public async Task<ActionResult<object>> GetHistorialCliente360(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound(new { message = "Cliente no encontrado." });

            var proformas = await _context.Proformas
                .Include(p => p.Paquetes)
                .Where(p => p.ClienteId == id)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            decimal totalFacturadoUSD = proformas.Where(p => p.Estado == "FACTURADO").Sum(p => p.TotalCobradoUSD);
            decimal saldoPendienteUSD = proformas.Where(p => p.Estado == "PENDIENTE_PAGO").Sum(p => p.TotalCobradoUSD);
            decimal totalLibrasMovidas = proformas.Sum(p => p.TotalLbs);
            int totalPaquetes = proformas.Sum(p => p.Paquetes.Count);

            return Ok(new
            {
                Cliente = new
                {
                    cliente.Id,
                    cliente.Nombre,
                    cliente.Telefono,
                    cliente.TipoCliente,
                    cliente.TarifaAereo,
                    cliente.TarifaMaritimo,
                    cliente.TarifaCelularFija,
                    cliente.TarifaTvAereo,
                    cliente.TarifaTvMaritimo
                },
                Metricas = new
                {
                    TotalFacturadoUSD = totalFacturadoUSD,
                    SaldoPendienteUSD = saldoPendienteUSD,
                    TotalLibrasMovidas = totalLibrasMovidas,
                    TotalPaquetesMovidos = totalPaquetes,
                    FrecuenciaEnvios = proformas.Count
                },
                UltimasProformas = proformas.Take(15).Select(p => new
                {
                    p.Id,
                    p.NumeroProforma,
                    p.Estado,
                    p.TotalLbs,
                    p.TotalCobradoUSD,
                    p.FechaRegistro,
                    p.FechaFacturacion
                })
            });
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null || !cliente.Activo)
                return NotFound(new { message = "Cliente no encontrado" });

            if (!await TieneAccesoClienteAsync(cliente.SucursalId))
                return Forbid();

            return Ok(new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                CodigoPais = cliente.CodigoPais,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                TarifaAereo = cliente.TarifaAereo,
                TarifaMaritimo = cliente.TarifaMaritimo,
                Direccion = cliente.Direccion,
                TipoCliente = cliente.TipoCliente,
                Activo = cliente.Activo,
                FechaRegistro = cliente.FechaRegistro
            });
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente([FromBody] ClienteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            int targetSucursalId = miSucursalId;

            if (userRole == "SUPER_ADMIN" && dto.SucursalId.HasValue)
            {
                var sucursalDestino = await _context.Sucursales.FindAsync(dto.SucursalId.Value);
                if (sucursalDestino == null || !sucursalDestino.Activa)
                    return BadRequest(new { message = "La sucursal seleccionada no existe o está inactiva." });

                if (sucursalDestino.TipoSucursal == "FRANQUICIA_B2B")
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No se pueden crear clientes en franquicias B2B." });

                targetSucursalId = dto.SucursalId.Value;
            }

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            var nuevoCliente = new Cliente
            {
                SucursalId = targetSucursalId,
                Nombre = dto.Nombre.Trim(),
                CodigoPais = dto.CodigoPais,
                Telefono = dto.Telefono.Trim(),
                Email = dto.Email?.Trim(),
                TarifaAereo = dto.TarifaAereo,
                TarifaMaritimo = dto.TarifaMaritimo,
                Direccion = dto.Direccion?.Trim(),
                TipoCliente = dto.TipoCliente,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Clientes.Add(nuevoCliente);

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = targetSucursalId,
                UsuarioId = usuarioId,
                Accion = "CREACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} registró al nuevo cliente '{nuevoCliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var response = new ClienteResponseDto
            {
                Id = nuevoCliente.Id,
                Nombre = nuevoCliente.Nombre,
                CodigoPais = nuevoCliente.CodigoPais,
                Telefono = nuevoCliente.Telefono,
                Email = nuevoCliente.Email,
                TarifaAereo = nuevoCliente.TarifaAereo,
                TarifaMaritimo = nuevoCliente.TarifaMaritimo,
                Direccion = nuevoCliente.Direccion,
                TipoCliente = nuevoCliente.TipoCliente,
                Activo = nuevoCliente.Activo,
                FechaRegistro = nuevoCliente.FechaRegistro
            };

            return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.Id }, response);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            if (!await TieneAccesoClienteAsync(cliente.SucursalId))
                return Forbid();

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            cliente.Nombre = dto.Nombre.Trim();
            cliente.CodigoPais = dto.CodigoPais;
            cliente.Telefono = dto.Telefono.Trim();
            cliente.Email = dto.Email?.Trim();
            cliente.TarifaAereo = dto.TarifaAereo;
            cliente.TarifaMaritimo = dto.TarifaMaritimo;
            cliente.Direccion = dto.Direccion?.Trim();
            cliente.TipoCliente = dto.TipoCliente;
            cliente.Activo = dto.Activo;

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = cliente.SucursalId,
                UsuarioId = usuarioId,
                Accion = "MODIFICACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} actualizó la información del cliente '{cliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            if (!await TieneAccesoClienteAsync(cliente.SucursalId))
                return Forbid();

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int usuarioId = int.TryParse(usuarioIdClaim, out int uId) ? uId : 1;
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Operador";

            cliente.Activo = false;

            _context.LogsAuditoria.Add(new LogAuditoria
            {
                SucursalId = cliente.SucursalId,
                UsuarioId = usuarioId,
                Accion = "ELIMINACION",
                Modulo = "CLIENTES",
                Descripcion = $"El usuario {nombreUsuario} desactivó al cliente '{cliente.Nombre}'.",
                FechaMovimiento = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> TieneAccesoClienteAsync(int clienteSucursalId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var sucursalCliente = await _context.Sucursales.FindAsync(clienteSucursalId);
            if (sucursalCliente == null) return false;

            if (userRole == "SUPER_ADMIN")
                return sucursalCliente.TipoSucursal == "PROPIA";

            var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
            if (miSucursal?.TipoSucursal == "PROPIA")
                return sucursalCliente.TipoSucursal == "PROPIA";

            return clienteSucursalId == miSucursalId;
        }
    }
}