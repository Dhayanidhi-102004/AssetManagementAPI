using AssetManagementAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetManagementAPI.Models;
using System.Security.Claims;
using AssetManagementAPI.Services;
namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetRequestController : ControllerBase
    {
        
        private readonly IAssetRequestService _assetRequestService;
        public AssetRequestController(IAssetRequestService assetRequestService)
        {
            _assetRequestService = assetRequestService;
        }
        [Authorize(Roles = "Employee")]
        [HttpPost("request/{id}")]
        public IActionResult CreateAssetRequest(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized("Invalid token");
            }
            var created =_assetRequestService.CreateAssetRequest(id, email);

            if (!created)
            {
                return BadRequest("Unable to create asset request");
            }

            return Ok("Asset request created successfully");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public IActionResult GetPendingRequests(int page=1,int pageSize=5)
        {
            var pendingRequests = _assetRequestService.GetPendingRequests(page,pageSize);
            return Ok(pendingRequests);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public IActionResult ApproveRequests(int id)
        {
           if(!_assetRequestService.ApproveAssetRequest(id))
            {
                return BadRequest("Request Cannot be approved");
            }
            return Ok("AssetRequest approved successfully");
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public IActionResult RejectPendingRequests(int id)
        {
            if (!_assetRequestService.RejectPendingRequests(id)) return BadRequest("Request rejection failed");
            return Ok("Asset request rejected successfully");
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("myrequests")]
        public IActionResult GetMyRequests(int page=1,int pageSize=5)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized("Invalid token");
            }
            
            return Ok(_assetRequestService.GetMyRequests(page,pageSize,email));
        }
    }
}
