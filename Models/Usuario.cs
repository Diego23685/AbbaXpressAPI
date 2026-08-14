using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbbaXpress.API.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Roles: SUPER_ADMIN, ADMIN_SUCURSAL, ADMIN_SUCURSAL_INDEPENDIENTE, OPERADOR, AUDITOR
        [Required]
        [MaxLength(40)]
        public string Rol { get; set; } = "OPERADOR";

        // Relación con Sucursal
        public int SucursalId { get; set; }

        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}