using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class GastoOperativoCreateDto
    {
        public int? SucursalId { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public string Categoria { get; set; } = "OTROS"; // RENTA, ENERGIA, AGUA, NOMINA, OTROS

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, 100000, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MontoUSD { get; set; }

        public decimal TipoCambio { get; set; } = 36.6243m;
        public string MetodoPago { get; set; } = "EFECTIVO_USD";
        public string? NumeroComprobante { get; set; }
        public DateTime? FechaGasto { get; set; }
    }

    public class GastoOperativoResponseDto
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal MontoUSD { get; set; }
        public decimal MontoNIO { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? NumeroComprobante { get; set; }
        public DateTime FechaGasto { get; set; }
    }

    public class ResumenFinancieroDto
    {
        public decimal IngresosTotalesUSD { get; set; }
        public decimal CostosProveedorUSD { get; set; }
        public decimal UtilidadBrutaUSD { get; set; }
        public decimal GastosOperativosTotalesUSD { get; set; }
        public decimal UtilidadNetaUSD { get; set; }
        public decimal UtilidadNetaNIO { get; set; }

        public Dictionary<string, decimal> DesgloseGastosUSD { get; set; } = new();
    }
}