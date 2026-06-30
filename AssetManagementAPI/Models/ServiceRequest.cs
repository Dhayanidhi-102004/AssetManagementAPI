using AssetManagementAPI.Enums;
namespace AssetManagementAPI.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int EmployeeId { get; set; }
        public IssueType IssueType { get; set; } 
        public string Description { get; set; } = string.Empty;
        public ServiceRequestStatus Status {  get; set; } = ServiceRequestStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }
}
