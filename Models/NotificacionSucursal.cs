using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class NotificacionSucursal
    {
        [Key]
        public int Id { get; set; }

        public int SucursalDestinoId { get; set; }
        [ForeignKey("SucursalDestinoId")]
        public Sucursal? SucursalDestino { get; set; }

        public int? ProformaId { get; set; }
        [ForeignKey("ProformaId")]
        public Proforma? Proforma { get; set; }

        [Required]
        [MaxLength(255)]
        public string Mensaje { get; set; } = string.Empty;

        public bool Leida { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}