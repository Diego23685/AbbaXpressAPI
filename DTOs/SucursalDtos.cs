using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class SucursalCreateDto
    {
        [Required(ErrorMessage = "El nombre de la sucursal es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [MaxLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(25)]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El tipo de sucursal es obligatorio.")]
        public string TipoSucursal { get; set; } = "FRANQUICIA_B2B"; // "PROPIA" o "FRANQUICIA_B2B"
    }

    public class SucursalUpdateDto
    {
        [Required(ErrorMessage = "El nombre de la sucursal es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [MaxLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(25)]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El tipo de sucursal es obligatorio.")]
        public string TipoSucursal { get; set; } = "FRANQUICIA_B2B";
    }
}