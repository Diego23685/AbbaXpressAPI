using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class ItemExportacionCreateDto
    {
        [Required]
        public string DescripcionES { get; set; } = string.Empty;

        [Required]
        public string DescripcionEN { get; set; } = string.Empty;

        public int Cantidad { get; set; } = 1;
        public decimal PesoLbs { get; set; }
        public decimal ValorDeclaradoUSD { get; set; }
    }

    public class EnvioExportacionCreateDto
    {
        [Required]
        public string RemitenteNombre { get; set; } = string.Empty;
        public string RemitenteTelefono { get; set; } = string.Empty;
        public string RemitenteDireccion { get; set; } = string.Empty;

        [Required]
        public string DestinatarioNombre { get; set; } = string.Empty;
        public string DestinatarioTelefono { get; set; } = string.Empty;
        [Required]
        public string DestinatarioEstado { get; set; } = "FL";
        public string DestinatarioCiudad { get; set; } = string.Empty;
        public string DestinatarioZipCode { get; set; } = string.Empty;
        public string DestinatarioDireccion { get; set; } = string.Empty;

        public decimal PesoTotalLbs { get; set; }
        public decimal? TarifaManualUSD { get; set; }
        public string? TrackingFedEx { get; set; }

        public List<ItemExportacionCreateDto> Items { get; set; } = new();
    }

    public class EnvioExportacionResponseDto
    {
        public int Id { get; set; }
        public string CodigoEnvio { get; set; } = string.Empty;
        public string? TrackingFedEx { get; set; }
        public string RemitenteNombre { get; set; } = string.Empty;
        public string RemitenteTelefono { get; set; } = string.Empty;
        public string DestinatarioNombre { get; set; } = string.Empty;
        public string DestinatarioEstado { get; set; } = string.Empty;
        public string DestinatarioCiudad { get; set; } = string.Empty;
        public decimal PesoTotalLbs { get; set; }
        public decimal TarifaBaseUSD { get; set; }
        public decimal RecargoEstadoUSD { get; set; }
        public decimal TotalCobradoUSD { get; set; }
        public decimal TotalCobradoNIO { get; set; }
        public string EstadoOperativo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public List<ItemExportacionCreateDto> Items { get; set; } = new();
    }
}