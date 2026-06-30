namespace AssetManagementAPI.DTOs.ServiceRequest
{
    public class ServiceRequestResponseDTO
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string AssetName { get; set; } = string.Empty;

        public string IssueType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ResolvedDate { get; set; }
    }
}