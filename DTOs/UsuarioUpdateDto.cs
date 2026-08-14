using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido.")]
        public string Rol { get; set; } = string.Empty;

        public int SucursalId { get; set; }

        // Opcional: solo se actualiza si viene con texto
        public string? Password { get; set; }
    }
}