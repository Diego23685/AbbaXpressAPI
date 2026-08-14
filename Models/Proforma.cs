using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Proforma
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string NumeroProforma { get; set; } = string.Empty; // Ej: "ABBA-1001"

        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public int SucursalOrigenId { get; set; }
        [ForeignKey("SucursalOrigenId")]
        public Sucursal? SucursalOrigen { get; set; }

        public int SucursalDestinoId { get; set; }
        [ForeignKey("SucursalDestinoId")]
        public Sucursal? SucursalDestino { get; set; }

        public int UsuarioCreacionId { get; set; }
        [ForeignKey("UsuarioCreacionId")]
        public Usuario? UsuarioCreacion { get; set; }

        // Estados: "PENDIENTE_PAGO", "FACTURADO", "EN_TRANSITO", "ENTREGADO", "ANULADO"
        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "PENDIENTE_PAGO";

        // Método de Pago: "CREDITO", "EFECTIVO_USD", "EFECTIVO_NIO", "TRANSFERENCIA", "POS"
        [MaxLength(30)]
        public string MetodoPago { get; set; } = "CREDITO";

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalLbs { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal CargoDeliveryUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DescuentoUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCobradoUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCostoProveedorUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,4)")]
        public decimal TipoCambioAplicado { get; set; } = 36.6243m; // T/C oficial del momento

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFacturacion { get; set; }

        // Relación 1:N con Paquetes
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}