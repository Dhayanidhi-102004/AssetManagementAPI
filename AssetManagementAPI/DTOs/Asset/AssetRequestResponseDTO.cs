namespace AssetManagementAPI.DTOs.Asset
{
    public class AssetRequestResponseDTO
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
