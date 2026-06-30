using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.DTOs.ServiceRequest;
using AssetManagementAPI.Enums;
using AssetManagementAPI.Models;

namespace AssetManagementAPI.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ServiceRequestService> _logger;

        public ServiceRequestService(
            AppDbContext context,
            ILogger<ServiceRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool CreateServiceRequest(
            CreateServiceRequestDTO dto,
            string email)
        {
            var employee=_context.Employees.FirstOrDefault(x => x.Email == email);
            if (employee == null)
            {
                _logger.LogWarning("Employee with email {Email} not found",email);
                return false;
            }
            var asset = _context.Assets.Find(dto.AssetId);
            if (asset == null) {
                _logger.LogWarning("Asset with Id {AssetId} not found", dto.AssetId);
                return false;
            }
            var allocation=_context.AssetAllocations.FirstOrDefault(a=>a.AssetId == dto.AssetId && a.EmployeeId==employee.Id&&a.Status=="Active");
            if (allocation==null)
            {
                _logger.LogWarning("Asset {AssetId} is not assigned to employee {EmployeeId}", dto.AssetId,employee.Id);
                return false;
            }
            bool alreadyPending = _context.ServiceRequests.Any(sr=> sr.AssetId == dto.AssetId && sr.EmployeeId==employee.Id && sr.Status == ServiceRequestStatus.Pending);
            if(alreadyPending)
            {
                _logger.LogWarning("Pending service request already exists for Asset {AssetId}", dto.AssetId);
                return false;
            }
            var request = new ServiceRequest
            {
                AssetId = dto.AssetId,
                EmployeeId = employee.Id,
                IssueType = dto.IssueType,
                Description = dto.Description,
                Status = ServiceRequestStatus.Pending,
                CreatedAt = DateTime.Now
            };
            _context.ServiceRequests.Add(request);
            _context.SaveChanges();
            _logger.LogInformation("Service request created successfully for Asset {AssetId}",dto.AssetId);

            return true;
        }

        public PagedResponseDTO<ServiceRequestResponseDTO>
    GetMyServiceRequests(
        int page,
        int pageSize,
        string email)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee with email {Email} not found",
                    email);

                return new PagedResponseDTO<ServiceRequestResponseDTO>
                {
                    TotalRecords = 0,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = 0,
                    Data = new List<ServiceRequestResponseDTO>()
                };
            }

            var query = _context.ServiceRequests
                .Where(sr => sr.EmployeeId == employee.Id);

            var totalRecords = query.Count();

            var requests = query
                .OrderByDescending(sr => sr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = requests
                .Select(MapServiceRequest)
                .ToList();

            return new PagedResponseDTO<ServiceRequestResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),
                Data = response
            };
        }

        public PagedResponseDTO<ServiceRequestResponseDTO>
            GetAllServiceRequests(
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
            var query = _context.ServiceRequests.AsQueryable();
            var totalRecords = query.Count();  
            var requests = query.OrderByDescending(sr=>sr.CreatedAt)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                .ToList();
            var response = requests.Select(MapServiceRequest).ToList();
            return new PagedResponseDTO<ServiceRequestResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
            (double)totalRecords / pageSize),
                Data = response
            };

        }

        public bool UpdateServiceRequestStatus(
    int id,
    ServiceRequestStatus status)
        {
            var request = _context.ServiceRequests
                .Find(id);

            if (request == null)
            {
                _logger.LogWarning(
                    "Service request with Id {Id} not found",
                    id);

                return false;
            }

            if (request.Status == ServiceRequestStatus.Resolved)
            {
                _logger.LogWarning(
                    "Service request {Id} already resolved",
                    id);

                return false;
            }

            if (request.Status == ServiceRequestStatus.Rejected)
            {
                _logger.LogWarning(
                    "Service request {Id} already rejected",
                    id);

                return false;
            }

            request.Status = status;

            if (status == ServiceRequestStatus.Resolved)
            {
                request.ResolvedDate = DateTime.Now;
            }

            _context.SaveChanges();

            _logger.LogInformation(
                "Service request {Id} status updated to {Status}",
                id,
                status);

            return true;
        }
        private ServiceRequestResponseDTO MapServiceRequest(
    ServiceRequest request)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Id == request.EmployeeId);

            var asset = _context.Assets
                .FirstOrDefault(a => a.Id == request.AssetId);

            return new ServiceRequestResponseDTO
            {
                Id = request.Id,
                EmployeeName = employee?.Name ?? "Unknown",
                AssetName = asset?.AssetName ?? "Unknown",
                IssueType = request.IssueType.ToString(),
                Description = request.Description,
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                ResolvedDate = request.ResolvedDate
            };
        }
    }

}