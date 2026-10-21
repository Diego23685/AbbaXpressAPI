using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return Ok(await _context.Proveedores.Where(p => p.Activo).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE")]
        public async Task<ActionResult<Proveedor>> CreateProveedor([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProveedores), new { id = proveedor.Id }, proveedor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SUPER_ADMIN,ADMIN_SUCURSAL_INDEPENDIENTE")]
        public async Task<IActionResult> UpdateProveedor(int id, [FromBody] Proveedor prov)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound();

            p.Nombre = prov.Nombre;
            p.Contacto = prov.Contacto;
            p.Telefono = prov.Telefono;
            p.CostoLibraAereo = prov.CostoLibraAereo;
            p.CostoLibraMaritimo = prov.CostoLibraMaritimo;
            p.Activo = prov.Activo;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}