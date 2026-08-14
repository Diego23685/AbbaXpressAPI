using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        // Sede a la que pertenece el cliente
        public int SucursalId { get; set; } = 1;
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string CodigoPais { get; set; } = "+505";

        [Required]
        public string Telefono { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaAereo { get; set; } = 7.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaMaritimo { get; set; } = 4.00m;

        public string? Direccion { get; set; }

        public string TipoCliente { get; set; } = "CONSUMIDOR_FINAL";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}