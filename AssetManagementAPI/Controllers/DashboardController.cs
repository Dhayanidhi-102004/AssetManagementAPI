using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        public IActionResult GetDashboardStats()
        {
            var stats = new
            {
                TotalAssets = _context.Assets.Count(),

                AvailableAssets = _context.Assets
                    .Count(a => a.Status == "Available"),

                AllocatedAssets = _context.AssetAllocations
                    .Count(a => a.Status == "Active"),

                ReturnedAssets = _context.AssetAllocations
                    .Count(a => a.Status == "Returned"),

                TotalEmployees = _context.Employees.Count(),

                PendingRequests = _context.AssetRequests
                    .Count(r => r.Status == "Pending")
            };

            return Ok(stats);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public IActionResult FilterAssetSearch(
            string? category,
            string? status,
            string? assetName,
            int page = 1,
            int pageSize = 5)
        {
            var query = _context.Assets.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category == category);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrEmpty(assetName))
            {
                query = query.Where(a =>
                    a.AssetName.Contains(assetName));
            }

            var totalRecords = query.Count();

            
            var assets = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            
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
                        assignedEmployee = new EmployeeResponseDTO
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            Email = employee.Email,
                            Role = employee.Role,
                            Gender = employee.Gender,
                            ContactNumber = employee.ContactNumber,
                            Address = employee.Address
                        };
                    }
                }

                return new AssetResponseDTO
                {
                    Id = asset.Id,
                    AssetName = asset.AssetName,
                    Category = asset.Category,
                    Status = asset.Status,
                    AssignedTo = assignedEmployee
                };
            }).ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                Data = response
            });
        }
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Testing middleware");
        }
    }
}