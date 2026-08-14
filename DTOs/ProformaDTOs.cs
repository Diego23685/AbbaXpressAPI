using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class PaqueteItemCreateDto
    {
        [Required(ErrorMessage = "El tracking es obligatorio")]
        public string Tracking { get; set; } = string.Empty;

        public string? Label { get; set; }

        [Range(0, 10000, ErrorMessage = "El peso debe ser mayor o igual a 0")]
        public decimal PesoLbs { get; set; } = 0.00m;

        // "AEREO" o "MARITIMO"
        public string ViaEnvio { get; set; } = "AEREO";

        // "GENERAL", "SMART_TV", "CELULAR", "PALLET"
        public string Categoria { get; set; } = "GENERAL";

        public decimal? TarifaManual { get; set; } // Opcional: si el operador la sobreescribe
        public decimal? CostoProveedorManual { get; set; }
    }

    public class ProformaCreateDto
    {
        [Required(ErrorMessage = "Debe especificar el cliente")]
        public int ClienteId { get; set; }

        public int SucursalDestinoId { get; set; }

        public decimal CargoDeliveryUSD { get; set; } = 0.00m;
        public decimal DescuentoUSD { get; set; } = 0.00m;
        public decimal TipoCambio { get; set; } = 36.6243m;

        // "CREDITO", "EFECTIVO_USD", "EFECTIVO_NIO", "TRANSFERENCIA", "POS"
        public string MetodoPago { get; set; } = "CREDITO";

        [MinLength(1, ErrorMessage = "Debe incluir al menos un paquete")]
        public List<PaqueteItemCreateDto> Paquetes { get; set; } = new();
    }

    public class PaqueteResponseDto
    {
        public int Id { get; set; }
        public string Tracking { get; set; } = string.Empty;
        public string? Label { get; set; }
        public decimal PesoLbs { get; set; }
        public string ViaEnvio { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal TarifaAplicada { get; set; }
        public decimal CostoProveedor { get; set; }
        public decimal SubtotalUSD { get; set; }
    }

    public class ProformaResponseDto
    {
        public int Id { get; set; }
        public string NumeroProforma { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;
        public string SucursalOrigen { get; set; } = string.Empty;
        public string SucursalDestino { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public decimal TotalLbs { get; set; }
        public decimal CargoDeliveryUSD { get; set; }
        public decimal DescuentoUSD { get; set; }
        public decimal TotalCobradoUSD { get; set; }
        public decimal TotalCobradoNIO { get; set; }
        public decimal TotalCostoProveedorUSD { get; set; }
        public decimal UtilidadBrutaUSD { get; set; }
        public decimal TipoCambioAplicado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaFacturacion { get; set; }
        public List<PaqueteResponseDto> Paquetes { get; set; } = new();
    }
}