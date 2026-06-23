using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.Models;

namespace AssetManagementAPI.Services
{
    public class AssetRequestService : IAssetRequestService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AssetRequestService> _logger;
        private readonly IAuditService _auditService;
        public AssetRequestService(AppDbContext context, ILogger<AssetRequestService> logger, IAuditService auditService)
        {
            _context = context;
            _logger = logger;
            _auditService = auditService;
        }

        public bool ApproveAssetRequest(int id)
        {
            try
            {
                var request = _context.AssetRequests.Find(id);
                if (request == null)
                {
                    _logger.LogWarning("Request with Id {RequestId} not found",
            id);
                    return false;
                }
                if (request.Status != "Pending")
                {
                    _logger.LogWarning("Request {RequestId} is not pending",
            id);
                    return false;
                }
                var asset = _context.Assets.Find(request.AssetId);
                if (asset == null)
                {
                    _logger.LogWarning("Asset with Id {AssetId} not found", request.AssetId);
                    return false;
                }

                if (asset.Status != "Pending")
                {
                    _logger.LogWarning("Asset {AssetId} status is {Status}",
                        asset.Id, asset.Status);
                    return false;
                }
                request.Status = "Approved";
                asset.Status = "Assigned";
                var allocation = new AssetAllocation
                {
                    AssetId = request.AssetId,
                    EmployeeId = request.EmployeeId,
                    AssignedDate = DateTime.Now,
                    Status = "Active"
                };

                _context.AssetAllocations.Add(allocation);
                _context.SaveChanges();
                _auditService.Log("Request Approved", "Admin", $"Request {id} approved");
                _logger.LogInformation("Request {RequestId} approved successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while approving request {RequestId}",id);
                throw;
            }

        }

        public bool CreateAssetRequest(int assetId, string email)
        {
            try
            {
                var asset = _context.Assets.Find(assetId);

                if (asset == null)
                {
                    _logger.LogWarning("Asset with Id {AssetId} not found",
            assetId);
                    return false;
                }

                if (asset.Status != "Available")
                {
                    _logger.LogWarning("Asset {AssetId} is not available for request",
            assetId);
                    return false;
                }

                var employee = _context.Employees
                    .FirstOrDefault(e => e.Email == email);

                if (employee == null)
                {
                    _logger.LogWarning("Employee with Email {Email} not found",
            email);
                    return false;
                }

                bool alreadyRequested = _context.AssetRequests.Any(ar =>
                    ar.AssetId == assetId &&
                    ar.EmployeeId == employee.Id &&
                    ar.Status == "Pending");

                if (alreadyRequested)
                {
                    _logger.LogWarning("Employee {EmployeeId} already requested Asset {AssetId}",
            employee.Id,
            assetId);
                    return false;
                }

                var request = new AssetRequest
                {
                    AssetId = assetId,
                    EmployeeId = employee.Id,
                    Status = "Pending",
                    RequestDate = DateTime.Now
                };

                _context.AssetRequests.Add(request);
                asset.Status = "Pending";
                _context.SaveChanges();
                _logger.LogInformation("Asset request created successfully. AssetId: {AssetId}, EmployeeId: {EmployeeId}",
        assetId,
        employee.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset request for Asset {AssetId}", assetId);
                throw;
            }
        }

        public PagedResponseDTO<AssetRequestResponseDTO> GetMyRequests(int page,int pageSize,string email)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }
            _logger.LogInformation("Fetching requests for employee {Email}",email);

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null)
            {
                _logger.LogWarning("Employee with Email {Email} not found",email);
                return new PagedResponseDTO<AssetRequestResponseDTO>
                {
                    TotalRecords = 0,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = 0,
                    Data = new List<AssetRequestResponseDTO>()
                };
            }

            var query = _context.AssetRequests
                .Where(ar => ar.EmployeeId == employee.Id);

            var totalRecords = query.Count();

            var requests = query
                .OrderByDescending(ar => ar.RequestDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var response = requests
    .Select(MapAssetRequest)
    .ToList();

            return new PagedResponseDTO<AssetRequestResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),

                Data = response
            };
        }

        public PagedResponseDTO<AssetRequestResponseDTO> GetPendingRequests(int page, int pageSize)
        {
            _logger.LogInformation("Fetching pending requests. Page: {Page}, PageSize: {PageSize}",page,pageSize);
            var query = _context.AssetRequests.Where(ar => ar.Status == "Pending");
            var totalRecords = query.Count();
            var requests = query
                    .OrderByDescending(ar => ar.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            var response = requests
    .Select(MapAssetRequest)
    .ToList();
            return new PagedResponseDTO<AssetRequestResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
        (double)totalRecords / pageSize),
                Data = response
            };
        }

        public bool RejectPendingRequests(int id)
        {
            try
            {
                var request = _context.AssetRequests.Find(id);
                if (request == null)
                {
                    _logger.LogWarning("Request {RequestId} not found",
            id);
                    return false;
                }
                if (request.Status != "Pending")
                {
                    _logger.LogWarning("Request {RequestId} is not pending", id);
                    return false;
                }
                request.Status = "Rejected";
                var asset = _context.Assets.Find(request.AssetId);

                if (asset != null)
                {
                    asset.Status = "Available";
                }
                _context.SaveChanges();
                _auditService.Log("Request Rejected", "Admin", $"Request {id} rejected");
                _logger.LogInformation("Request {RequestId} rejected successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error rejecting request {RequestId}", id);
                throw;
            }
        }
        private AssetRequestResponseDTO MapAssetRequest(
    AssetRequest request)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Id == request.EmployeeId);

            var asset = _context.Assets
                .FirstOrDefault(a => a.Id == request.AssetId);

            return new AssetRequestResponseDTO
            {
                Id = request.Id,
                EmployeeName = employee?.Name ?? "Unknown",
                AssetName = asset?.AssetName ?? "Unknown",
                RequestDate = request.RequestDate,
                Status = request.Status
            };
        }
    }
}
