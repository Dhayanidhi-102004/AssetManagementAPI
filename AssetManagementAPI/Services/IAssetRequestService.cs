using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.Models;

namespace AssetManagementAPI.Services
{
    public interface IAssetRequestService
    {
        bool CreateAssetRequest(int id, string email);
        PagedResponseDTO<AssetRequestResponseDTO> GetPendingRequests(
    int page,
    int pageSize);
        bool ApproveAssetRequest(int id);
        bool RejectPendingRequests(int id);
        PagedResponseDTO<AssetRequestResponseDTO> GetMyRequests(
    int page,
    int pageSize,string email);
    }
}
