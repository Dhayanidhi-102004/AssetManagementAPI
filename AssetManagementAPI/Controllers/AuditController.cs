using AssetManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(
        IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public IActionResult GetAuditLogs(
        string? action,
        string? sortDirection,
        int page = 1,
        int pageSize = 5)
    {
        return Ok(
            _auditService.GetAuditLogs(
                action,
                sortDirection,
                page,
                pageSize));
    }
}