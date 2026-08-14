using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Paquete
    {
        [Key]
        public int Id { get; set; }

        public int ProformaId { get; set; }
        [ForeignKey("ProformaId")]
        public Proforma? Proforma { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tracking { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Label { get; set; } // Rótulo o descripción personalizada del ítem

        [Column(TypeName = "decimal(10,2)")]
        public decimal PesoLbs { get; set; } = 0.00m;

        // "AEREO" o "MARITIMO"
        [Required]
        [MaxLength(20)]
        public string ViaEnvio { get; set; } = "AEREO";

        // Categoría: "GENERAL", "SMART_TV", "CELULAR", "PALLET"
        [Required]
        [MaxLength(30)]
        public string Categoria { get; set; } = "GENERAL";

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaAplicada { get; set; } = 0.00m; // $/lb o tarifa plana ($35 cel)

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoProveedor { get; set; } = 0.00m; // Costo directo AereoMar

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubtotalUSD { get; set; } = 0.00m; // Monto final facturado del paquete
    }
}