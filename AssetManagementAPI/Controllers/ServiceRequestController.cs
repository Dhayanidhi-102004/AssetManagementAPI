using AssetManagementAPI.DTOs.ServiceRequest;
using AssetManagementAPI.Enums;
using AssetManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(
            IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public IActionResult CreateServiceRequest(
            CreateServiceRequestDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(
                    "Service request data is required.");
            }

            var email = User
                .FindFirst(ClaimTypes.Email)
                ?.Value;

            if (email == null)
            {
                return Unauthorized("Invalid token");
            }

            var created =
                _serviceRequestService
                .CreateServiceRequest(dto, email);

            if (!created)
            {
                return BadRequest(
                    "Unable to create service request");
            }

            return Ok(
                "Service request created successfully");
        }

        [Authorize(Roles = "Employee")]
        [HttpGet("myrequests")]
        public IActionResult GetMyServiceRequests(
            int page = 1,
            int pageSize = 5)
        {
            var email = User
                .FindFirst(ClaimTypes.Email)
                ?.Value;

            if (email == null)
            {
                return Unauthorized("Invalid token");
            }

            var result =
                _serviceRequestService
                .GetMyServiceRequests(
                    page,
                    pageSize,
                    email);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAllServiceRequests(
            int page = 1,
            int pageSize = 5)
        {
            var result =
                _serviceRequestService
                .GetAllServiceRequests(
                    page,
                    pageSize);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("status/{id}")]
        public IActionResult UpdateServiceRequestStatus(
            int id,
            ServiceRequestStatus status)
        {
            var updated =
                _serviceRequestService
                .UpdateServiceRequestStatus(
                    id,
                    status);

            if (!updated)
            {
                return BadRequest(
                    "Status update failed");
            }

            return Ok(
                "Service request status updated successfully");
        }
    }
}