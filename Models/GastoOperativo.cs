using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class GastoOperativo
    {
        [Key]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // Categoría: "RENTA", "ENERGIA", "AGUA", "NOMINA", "OTROS"
        [Required]
        [MaxLength(30)]
        public string Categoria { get; set; } = "OTROS";

        [Required]
        [MaxLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontoUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,4)")]
        public decimal TipoCambioAplicado { get; set; } = 36.6243m;

        [MaxLength(30)]
        public string MetodoPago { get; set; } = "EFECTIVO_USD"; // EFECTIVO_USD, EFECTIVO_NIO, TRANSFERENCIA

        public DateTime FechaGasto { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? NumeroComprobante { get; set; }
    }
}