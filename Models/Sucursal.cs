using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Sucursal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Ciudad { get; set; } = "Managua";

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(25)]
        public string? Telefono { get; set; }

        // Tipo: "PROPIA" (Bolonia, Doral) o "FRANQUICIA_B2B" (León)
        [Required]
        [MaxLength(30)]
        public string TipoSucursal { get; set; } = "PROPIA";

        public bool Activa { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relaciones de navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}