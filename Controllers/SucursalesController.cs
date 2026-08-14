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
            else // Managua solo ve Bolonia (1) y Doral (2)
            {
                query = query.Where(s => s.Id == 1 || s.Id == 2);
            }

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