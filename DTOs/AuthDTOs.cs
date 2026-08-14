using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
    }

    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El username es obligatorio")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        // Roles: SUPER_ADMIN, ADMIN_SUCURSAL, ADMIN_SUCURSAL_INDEPENDIENTE, OPERADOR, AUDITOR
        [Required]
        public string Rol { get; set; } = "OPERADOR";

        [Required(ErrorMessage = "Debe asignar una sucursal")]
        public int SucursalId { get; set; }
    }
}