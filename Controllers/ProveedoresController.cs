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
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
            bool esPropia = miSucursal?.TipoSucursal == "PROPIA";

            var query = _context.Proveedores
                .Include(p => p.Sucursal)
                .Where(p => p.Activo)
                .AsQueryable();

            if (esPropia)
            {
                query = query.Where(p => p.Sucursal != null && p.Sucursal.TipoSucursal == "PROPIA");
            }
            else
            {
                query = query.Where(p => p.SucursalId == miSucursalId);
            }

            return Ok(await query.ToListAsync());
        }

        // POST: api/proveedores
        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE,ADMIN_SUCURSAL")]
        public async Task<ActionResult<Proveedor>> CreateProveedor([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            proveedor.SucursalId = miSucursalId;
            proveedor.Activo = true;
            proveedor.FechaRegistro = DateTime.UtcNow;

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedores), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE,ADMIN_SUCURSAL")]
        public async Task<IActionResult> UpdateProveedor(int id, [FromBody] Proveedor dto)
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var prov = await _context.Proveedores.Include(p => p.Sucursal).FirstOrDefaultAsync(p => p.Id == id);
            if (prov == null) return NotFound();

            var miSucursal = await _context.Sucursales.FindAsync(miSucursalId);
            bool esPropia = miSucursal?.TipoSucursal == "PROPIA";

            if (!esPropia && prov.SucursalId != miSucursalId)
                return Forbid();

            prov.Nombre = dto.Nombre;
            prov.Contacto = dto.Contacto;
            prov.Telefono = dto.Telefono;
            prov.CostoLibraAereo = dto.CostoLibraAereo;
            prov.CostoLibraMaritimo = dto.CostoLibraMaritimo;
            prov.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return Ok(prov);
        }
    }
}