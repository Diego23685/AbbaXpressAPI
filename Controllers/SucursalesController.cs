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
    public class SucursalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetSucursales()
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            var query = _context.Sucursales.Where(s => s.Activa).AsQueryable();

            if (miSucursalId == 3) // León solo se ve a sí misma
            {
                query = query.Where(s => s.Id == 3);
            }
            // Managua (1 y 2) y Super Admin ven todas (Bolonia, Doral y León) para permitir traslados inter-sucursal

            return Ok(await query.OrderBy(s => s.Id).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sucursal>> GetSucursal(int id)
        {
            var sucursalClaim = User.FindFirstValue("SucursalId");
            int miSucursalId = int.TryParse(sucursalClaim, out int sId) ? sId : 1;

            // Bloquear consulta directa por ID cruzado
            if (miSucursalId == 3 && id != 3)
                return Forbid();

            if ((miSucursalId == 1 || miSucursalId == 2) && (id != 1 && id != 2))
                return Forbid();

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null || !sucursal.Activa)
                return NotFound(new { message = "Sucursal no encontrada" });

            return Ok(sucursal);
        }
    }
}