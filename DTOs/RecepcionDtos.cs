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

}