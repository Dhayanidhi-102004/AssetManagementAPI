using System.ComponentModel.DataAnnotations;

namespace AssetManagementAPI.DTOs.Asset
{
    public class CreateAssetDTO
    {
        [Required]
        public string AssetName { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
    }
}
