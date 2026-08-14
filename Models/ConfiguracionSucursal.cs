using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class ConfiguracionSucursal
    {
        [Key]
        public int Id { get; set; }

        // Vinculación formal por sede
        [Required]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal TipoCambioNIO { get; set; } = 36.6243m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaAereoGeneral { get; set; } = 7.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaMaritimoGeneral { get; set; } = 4.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaCelularFija { get; set; } = 35.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaTvMaritimo { get; set; } = 3.50m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaTvAereo { get; set; } = 7.50m;

        // Costos base de flete proveedor
        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoProveedorAereo { get; set; } = 3.80m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoProveedorMaritimo { get; set; } = 1.50m;

        public DateTime UltimaModificacion { get; set; } = DateTime.UtcNow;
    }
}