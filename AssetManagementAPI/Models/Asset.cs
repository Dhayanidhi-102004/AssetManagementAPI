using System.ComponentModel.DataAnnotations;
namespace AssetManagementAPI.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AssetName { get; set; }=string.Empty;
        [Required]
        public string Category { get; set; }=string.Empty;
        [Required]
        public string SerialNumber { get; set; }=string.Empty;

        public string Status { get; set; }= "Available";

        public DateTime PurchaseDate { get; set; }
    }
}
