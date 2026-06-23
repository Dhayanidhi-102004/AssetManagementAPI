using System.ComponentModel.DataAnnotations;
namespace AssetManagementAPI.Models
{
    public class AssetRequest
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int AssetId { get; set; }
        public DateTime RequestDate { get; set; }= DateTime.Now;
        public string Status { get; set; } = "Pending";
    }
}
