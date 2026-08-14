using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string CodigoPais { get; set; } = "+505";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Telefono { get; set; } = string.Empty;

        public string? Email { get; set; }

        public decimal TarifaAereo { get; set; } = 7.00m;

        public decimal TarifaMaritimo { get; set; } = 4.00m;

        public string? Direccion { get; set; }

        public string TipoCliente { get; set; } = "CONSUMIDOR_FINAL";
    }

    public class ClienteUpdateDto : ClienteCreateDto
    {
        public bool Activo { get; set; } = true;
    }

    public class ClienteResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoPais { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public decimal TarifaAereo { get; set; }
        public decimal TarifaMaritimo { get; set; }
        public string? Direccion { get; set; }
        public string TipoCliente { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}