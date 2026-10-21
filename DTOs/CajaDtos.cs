using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class DenominacionItemDto
    {
        [Required]
        public string Moneda { get; set; } = "NIO"; // NIO o USD
        public decimal Denominacion { get; set; }
        public int Cantidad { get; set; }
    }

    public class AperturaCajaDto
    {
        public int? SucursalId { get; set; }
        public int BultosFisicosApertura { get; set; }
        public List<DenominacionItemDto> Denominaciones { get; set; } = new();
    }

    public class CierreCajaDto
    {
        public int BultosFisicosCierre { get; set; }
        public List<DenominacionItemDto> Denominaciones { get; set; } = new();
    }

    public class ValeCajaDto
    {
        [Required]
        [MaxLength(200)]
        public string Concepto { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Beneficiario { get; set; } = string.Empty;

        public decimal Monto { get; set; }

        [Required]
        public string Moneda { get; set; } = "NIO"; // NIO o USD
    }

    public class LiquidarProformaDto
    {
        [Required]
        public string MetodoPago { get; set; } = "EFECTIVO_NIO"; // EFECTIVO_NIO, EFECTIVO_USD, TRANSFERENCIA, POS

        public decimal CargoDeliveryUSD { get; set; } = 0.00m;
        public decimal DescuentoUSD { get; set; } = 0.00m;

        // Parámetros para pago POS
        public decimal PorcentajeComisionPOS { get; set; } = 0.00m;

        // Parámetros para cobro en Efectivo y Vuelto
        public decimal MontoRecibidoEfectivo { get; set; } = 0.00m;
        public string MonedaPagoEfectivo { get; set; } = "NIO"; // NIO o USD
    }
}