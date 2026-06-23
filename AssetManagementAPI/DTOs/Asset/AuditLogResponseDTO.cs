namespace AssetManagementAPI.DTOs.Asset
{
    public class AuditLogResponseDTO
    {
        public int Id { get; set; }

        public string Action { get; set; } = string.Empty;

        public string PerformedBy { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
