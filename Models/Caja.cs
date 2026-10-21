using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class SesionCaja
    {
        [Key]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        public int UsuarioAperturaId { get; set; }
        [ForeignKey("UsuarioAperturaId")]
        public Usuario? UsuarioApertura { get; set; }

        public int? UsuarioCierreId { get; set; }
        [ForeignKey("UsuarioCierreId")]
        public Usuario? UsuarioCierre { get; set; }

        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; set; }

        public int BultosFisicosApertura { get; set; } = 0;
        public int? BultosFisicosCierre { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoAperturaCordobas { get; set; } = 0.00m;

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoAperturaDolares { get; set; } = 0.00m;

        [Column(TypeName = "decimal(12,2)")]
        public decimal? MontoCierreCordobas { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? MontoCierreDolares { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "ABIERTA";

        public ICollection<DenominacionCaja> Denominaciones { get; set; } = new List<DenominacionCaja>();
        public ICollection<MovimientoCaja> Movimientos { get; set; } = new List<MovimientoCaja>();
    }

    public class DenominacionCaja
    {
        [Key]
        public int Id { get; set; }

        public int SesionCajaId { get; set; }
        [ForeignKey("SesionCajaId")]
        public SesionCaja? SesionCaja { get; set; }

        [Required]
        [MaxLength(10)]
        public string Momento { get; set; } = "APERTURA";

        [Required]
        [MaxLength(5)]
        public string Moneda { get; set; } = "NIO";

        [Column(TypeName = "decimal(10,2)")]
        public decimal Denominacion { get; set; }

        public int Cantidad { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal => Denominacion * Cantidad;
    }

    public class MovimientoCaja
    {
        [Key]
        public int Id { get; set; }

        public int SesionCajaId { get; set; }
        [ForeignKey("SesionCajaId")]
        public SesionCaja? SesionCaja { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(30)]
        public string TipoMovimiento { get; set; } = "EGRESO_VALE";

        [Required]
        [MaxLength(200)]
        public string Concepto { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? Beneficiario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; } = 0.00m;

        [MaxLength(5)]
        public string Moneda { get; set; } = "NIO";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}