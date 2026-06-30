using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.DTOs.ServiceRequest;
using AssetManagementAPI.Enums;

namespace AssetManagementAPI.Services
{
    public interface IServiceRequestService
    {
        bool CreateServiceRequest(
            CreateServiceRequestDTO dto,
            string email);

        PagedResponseDTO<ServiceRequestResponseDTO>
            GetMyServiceRequests(
                int page,
                int pageSize,
                string email);

        PagedResponseDTO<ServiceRequestResponseDTO>
            GetAllServiceRequests(
                int page,
                int pageSize);

        bool UpdateServiceRequestStatus(
            int id,
            ServiceRequestStatus status);
    }
}