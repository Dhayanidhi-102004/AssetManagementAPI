using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;

namespace AssetManagementAPI.Services
{
    public interface IAuditService
    {
        void Log(
            string action,
            string performedBy,
            string details);
        PagedResponseDTO<AuditLogResponseDTO> GetAuditLogs(string? action,
        string? sortDirection,
        int page,
        int pageSize);
    }
}