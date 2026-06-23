using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Common;
using AssetManagementAPI.Models;

namespace AssetManagementAPI.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public void Log(
            string action,
            string performedBy,
            string details)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                PerformedBy = performedBy,
                Details = details,
                CreatedAt = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
        public PagedResponseDTO<AuditLogResponseDTO> GetAuditLogs(
    string? action,
    string? sortDirection,
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

            var query = _context.AuditLogs.AsQueryable();

            // Filter
            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(a =>
                    a.Action.ToLower().Contains(action.ToLower()));
            }

            // Sort
            query = sortDirection?.ToLower() == "asc"
                ? query.OrderBy(a => a.CreatedAt)
                : query.OrderByDescending(a => a.CreatedAt);

            var totalRecords = query.Count();

            var logs = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = logs.Select(log => new AuditLogResponseDTO
            {
                Id = log.Id,
                Action = log.Action,
                PerformedBy = log.PerformedBy,
                Details = log.Details,
                CreatedAt = log.CreatedAt
            }).ToList();

            return new PagedResponseDTO<AuditLogResponseDTO>
            {
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),

                Data = response
            };
        }
    }
}