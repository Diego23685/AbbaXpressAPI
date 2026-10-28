using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        public int SucursalId { get; set; } = 1;

        [ForeignKey("SucursalId")]
        public virtual Sucursal? Sucursal { get; set; }

        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Contacto { get; set; }

        [MaxLength(25)]
        public string? Telefono { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoLibraAereo { get; set; } = 3.80m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoLibraMaritimo { get; set; } = 1.50m;

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}