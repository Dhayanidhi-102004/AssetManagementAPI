namespace AssetManagementAPI.DTOs.Asset
{
    public class AssetAllocationHistoryDTO
    {
        public string EmployeeName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}