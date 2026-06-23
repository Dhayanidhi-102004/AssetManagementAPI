using System.Globalization;
using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.DTOs.Employee;
using AssetManagementAPI.Models;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetManagementAPI.Services
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetService> _logger;
        private readonly IAuditService _auditService;

        public AssetService(AppDbContext context,IMapper mapper, ILogger<AssetService> logger, IAuditService auditService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }

        public string CreateAsset(CreateAssetDTO createAssetDTO)
        {
            try
            {
                var asset = _mapper.Map<Asset>(createAssetDTO);

                _context.Assets.Add(asset);
                _context.SaveChanges();
                _auditService.Log("Asset Created", "Admin", $"Asset {asset.AssetName} created");
                _logger.LogInformation("Asset {AssetName} created successfully", asset.AssetName);
                return "Asset created successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                throw;
            }
        }
        public AssetResponseDTO? GetAssetById(int id)
        {
            var asset = _context.Assets.Find(id);

            if (asset == null)
            {
                _logger.LogWarning("Asset with Id {Id} not found",id);
                return null;
            }

            var allocation = _context.AssetAllocations
                .FirstOrDefault(a => a.AssetId == asset.Id && a.Status == "Active");

            EmployeeResponseDTO? assignedEmployee = null;

            if (allocation != null)
            {
                var employee = _context.Employees
                    .FirstOrDefault(e => e.Id == allocation.EmployeeId);

                if (employee != null)
                {
                    assignedEmployee = _mapper.Map<EmployeeResponseDTO>(employee);
                }
            }

            var response= _mapper.Map<AssetResponseDTO>(asset);
            response.AssignedTo = assignedEmployee;
            _logger.LogInformation("Asset with {Id} retrived successfully", asset.Id);
            return response;
        }
        public bool DeleteAssetById(int id)
        {
            try
            {
                var asset = _context.Assets.Find(id);

                if (asset == null)
                {
                    _logger.LogWarning("Asset with Id {Id} not found", id);
                    return false;
                }

                _context.Assets.Remove(asset);
                _context.SaveChanges();
                _auditService.Log("Asset Deleted", "Admin", $"Asset {asset.AssetName} deleted");
                _logger.LogInformation("Asset with id {Id} deleted", asset.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {AssetId}", id);
                throw;
            }
        }
        public PagedResponseDTO<AssetResponseDTO> GetAllAssets(string? category,
                        string? status,string? sortBy,string? sortDirection,
                        int page,int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            var query = _context.Assets.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status.ToLower() == status.ToLower());
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "assetname":

                        query = sortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(a => a.AssetName)
                            : query.OrderBy(a => a.AssetName);

                        break;

                    case "purchasedate":

                        query = sortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(a => a.PurchaseDate)
                            : query.OrderBy(a => a.PurchaseDate);

                        break;

                    case "category":

                        query = sortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(a => a.Category)
                            : query.OrderBy(a => a.Category);

                        break;

                    case "status":

                        query = sortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(a => a.Status)
                            : query.OrderBy(a => a.Status);

                        break;
                }
            }
            else
            {
                query = query.OrderBy(a => a.Id);
            }

            // Count before pagination
            var totalRecords = query.Count();

            // Pagination
            var assets = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Mapping
            var response = assets.Select(asset =>
            {
                var allocation = _context.AssetAllocations
                    .FirstOrDefault(a =>
                        a.AssetId == asset.Id &&
                        a.Status == "Active");

                EmployeeResponseDTO? assignedEmployee = null;

                if (allocation != null)
                {
                    var employee = _context.Employees
                        .FirstOrDefault(e =>
                            e.Id == allocation.EmployeeId);

                    if (employee != null)
                    {
                        assignedEmployee =
                            _mapper.Map<EmployeeResponseDTO>(employee);
                    }
                }

                var assetDto =
                    _mapper.Map<AssetResponseDTO>(asset);

                assetDto.AssignedTo = assignedEmployee;

                return assetDto;
            }).ToList();

            return new PagedResponseDTO<AssetResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),

                Data = response
            };
        }
        public bool UpdateAsset(int id, UpdateAssetDTO asset)
        {
            try
            {
                var oldAsset = _context.Assets.Find(id);

                if (oldAsset == null)
                {
                    _logger.LogWarning(
            "Asset with Id {Id} not found for update",
            id);
                    return false;
                }

                _mapper.Map(asset, oldAsset);

                _context.SaveChanges();
                _auditService.Log("Asset Updated", "Admin", $"Asset {oldAsset.AssetName} updated");
                _logger.LogInformation("Asset with Id {Id} updated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating asset {AssetId}", id);
                throw;
            }
        }
        public bool ReturnAsset(int id)
        {
            try
            {
                var allocation = _context.AssetAllocations
                    .FirstOrDefault(a => a.AssetId == id && a.Status == "Active");

                if (allocation == null)
                {
                    _logger.LogWarning("No active allocation found for AssetId {AssetId}", id);
                    return false;
                }

                allocation.Status = "Returned";
                allocation.ReturnedDate = DateTime.Now;

                var asset = _context.Assets.Find(id);

                if (asset != null)
                {
                    _logger.LogWarning("Asset with Id {Id} not found", id);
                    asset.Status = "Available";
                }

                _context.SaveChanges();
                _auditService.Log("Asset Returned", "Employee", $"Asset {id} returned");
                _logger.LogInformation("Asset {AssetId} returned successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while returning asset {AssetId}", id);
                throw;
            }
        }
        public PagedResponseDTO<AssetAllocationHistoryDTO> GetAllocationHistory(
    int assetId,
    int page,
    int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }
            _logger.LogInformation("Fetching allocation history for AssetId {AssetId}",assetId);
            var query = _context.AssetAllocations
                .Where(a => a.AssetId == assetId);

            var totalRecords = query.Count();

            var allocations = query
                .OrderByDescending(a => a.AssignedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = allocations.Select(a =>
            {
                var employee = _context.Employees
                    .FirstOrDefault(e => e.Id == a.EmployeeId);

                return new AssetAllocationHistoryDTO
                {
                    EmployeeName = employee?.Name ?? "Unknown",
                    AssignedDate = a.AssignedDate,
                    ReturnedDate = a.ReturnedDate,
                    Status = a.Status
                };
            }).ToList();

            return new PagedResponseDTO<AssetAllocationHistoryDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),

                Data = response
            };
        }
        public PagedResponseDTO<AssetResponseDTO>
    GetAvailableAssets(int page, int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 5;
            var query = _context.Assets
                .Where(a => a.Status == "Available");

            var totalRecords = query.Count();

            var assets = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = _mapper
                .Map<List<AssetResponseDTO>>(assets);

            return new PagedResponseDTO<AssetResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),
                Data = response
            };
        }
        public List<AssetResponseDTO> GetMyAssets(string email)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null)
            {
                return new List<AssetResponseDTO>();
            }

            var allocations = _context.AssetAllocations
                .Where(a =>
                    a.EmployeeId == employee.Id &&
                    a.Status == "Active")
                .ToList();

            var assetIds = allocations
                .Select(a => a.AssetId)
                .ToList();

            var assets = _context.Assets
                .Where(a => assetIds.Contains(a.Id))
                .ToList();

            return _mapper.Map<List<AssetResponseDTO>>(assets);
        }
    }
}