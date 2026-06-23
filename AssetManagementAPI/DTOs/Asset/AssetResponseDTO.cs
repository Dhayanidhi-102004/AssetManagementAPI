using AssetManagementAPI.Models;
using AssetManagementAPI.DTOs.Employee;

namespace AssetManagementAPI.DTOs.Asset
{
    public class AssetResponseDTO
    {
        public int Id { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public EmployeeResponseDTO? AssignedTo { get; set; }
    }
}
