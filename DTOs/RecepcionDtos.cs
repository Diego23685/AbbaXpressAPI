using System.ComponentModel.DataAnnotations;

namespace AbbaXpress.API.DTOs
{
    public class PaqueteRecepcionItemDto
    {
        [Required]
        public string Tracking { get; set; } = string.Empty;
        public decimal PesoLbs { get; set; }
        public string ViaEnvio { get; set; } = "AEREO"; // AEREO, MARITIMO
        public string Categoria { get; set; } = "GENERAL"; // GENERAL, CELULAR, SMART_TV, PALLET
        public int? ProveedorId { get; set; }
        public decimal? TarifaManual { get; set; }
    }
    public class PaqueteUpdateDto
    {
        public int Id { get; set; }
        public string Tracking { get; set; } = string.Empty;
        public decimal PesoLbs { get; set; }
        public string ViaEnvio { get; set; } = "AEREO";
        public string Categoria { get; set; } = "GENERAL";
        public decimal TarifaAplicada { get; set; }
    }

    public class RecepcionCargaDto
    {
        [Required]
        public int ClienteId { get; set; }
        public int? SucursalDestinoId { get; set; }
        [Required]
        public List<PaqueteRecepcionItemDto> Paquetes { get; set; } = new();
    }
}