using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    [Table("LogsAuditoria")]
    public class LogAuditoria
    {
        [Key]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal? Sucursal { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string Accion { get; set; } = string.Empty; // CREACION, MODIFICACION, ELIMINACION, COBRO

        [Required]
        [MaxLength(100)]
        public string Modulo { get; set; } = string.Empty; // PROFORMAS, GASTOS, CLIENTES, USUARIOS

        [Required]
        public string Descripcion { get; set; } = string.Empty; // Ej. "El usuario Ricardo modificó..."

        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
    }
}