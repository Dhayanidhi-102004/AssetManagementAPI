using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.Models;

namespace AssetManagementAPI.Services
{
    public interface IAssetService
    {
        string CreateAsset(CreateAssetDTO createAssetDTO);

        AssetResponseDTO? GetAssetById(int id);

        PagedResponseDTO<AssetResponseDTO> GetAllAssets(
    string? category,
    string? status,
    string? sortBy,
    string? sortDirection,
    int page,
    int pageSize);

        bool DeleteAssetById(int id);

        bool UpdateAsset(int id, UpdateAssetDTO asset);

        bool ReturnAsset(int id);
        PagedResponseDTO<AssetAllocationHistoryDTO>
    GetAllocationHistory(
        int assetId,
        int page,
        int pageSize);
        PagedResponseDTO<AssetResponseDTO>
    GetAvailableAssets(int page, int pageSize);
        List<AssetResponseDTO> GetMyAssets(string email);
    }
}