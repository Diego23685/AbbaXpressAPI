using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class EnvioExportacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string CodigoEnvio { get; set; } = string.Empty; // Ej: "EXP-1001"

        [MaxLength(50)]
        public string? TrackingFedEx { get; set; }

        public int SucursalOrigenId { get; set; }
        [ForeignKey("SucursalOrigenId")]
        public Sucursal? SucursalOrigen { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // Datos del Remitente (Nicaragua)
        [Required]
        [MaxLength(150)]
        public string RemitenteNombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string RemitenteTelefono { get; set; } = string.Empty;

        [MaxLength(255)]
        public string RemitenteDireccion { get; set; } = string.Empty;

        // Datos del Destinatario (EE. UU.)
        [Required]
        [MaxLength(150)]
        public string DestinatarioNombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string DestinatarioTelefono { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DestinatarioEstado { get; set; } = "FL"; // FL, TX, CA, etc.

        [Required]
        [MaxLength(100)]
        public string DestinatarioCiudad { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DestinatarioZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string DestinatarioDireccion { get; set; } = string.Empty;

        // Métricas de Envío
        [Column(TypeName = "decimal(10,2)")]
        public decimal PesoTotalLbs { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaBaseUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal RecargoEstadoUSD { get; set; } = 0.00m; // +$10 si no es FL

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCobradoUSD { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,4)")]
        public decimal TipoCambioAplicado { get; set; } = 36.6243m;

        [Required]
        [MaxLength(30)]
        public string EstadoOperativo { get; set; } = "RECEPCIONADO_NICARAGUA"; 
        // RECEPCIONADO_NICARAGUA, EN_ESPERA_FEDEX, EN_TRANSITO_FEDEX, ENTREGADO

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relación 1:N con Ítems aduaneros
        public ICollection<ItemExportacion> Items { get; set; } = new List<ItemExportacion>();
    }

    public class ItemExportacion
    {
        [Key]
        public int Id { get; set; }

        public int EnvioExportacionId { get; set; }
        [ForeignKey("EnvioExportacionId")]
        public EnvioExportacion? EnvioExportacion { get; set; }

        [Required]
        [MaxLength(150)]
        public string DescripcionES { get; set; } = string.Empty; // Ej: "Queso seco artesanal"

        [Required]
        [MaxLength(150)]
        public string DescripcionEN { get; set; } = string.Empty; // Ej: "Artisanal dry cheese"

        public int Cantidad { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PesoLbs { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDeclaradoUSD { get; set; } = 0.00m;
    }
}