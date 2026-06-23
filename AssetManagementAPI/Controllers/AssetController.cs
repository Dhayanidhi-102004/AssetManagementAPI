using System.Security.Claims;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public IActionResult CreateAsset(CreateAssetDTO asset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_assetService.CreateAsset(asset));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetAssetById(int id)
        {
            var asset = _assetService.GetAssetById(id);

            if (asset == null)
            {
                return NotFound("Asset not found");
            }

            return Ok(asset);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteAsset(int id)
        {
            var deleted = _assetService.DeleteAssetById(id);

            if (!deleted)
            {
                return NotFound("Asset not found");
            }

            return Ok("Asset deleted successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public IActionResult GetAllAssets(string?category,string?status,
            string?sortBy,string?sortDirection,int page=1,int pageSize=5)
        {
            return Ok(_assetService.GetAllAssets(category,
        status,
        sortBy,
        sortDirection,
        page,
        pageSize));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateAsset(int id, UpdateAssetDTO updateAsset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = _assetService.UpdateAsset(id, updateAsset);

            if (!updated)
            {
                return NotFound("Asset not found");
            }

            return Ok("Asset updated successfully");
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("return/{assetId}")]
        public IActionResult ReturnAsset(int assetId)
        {
            var returned = _assetService.ReturnAsset(assetId);

            if (!returned)
            {
                return BadRequest("Asset is not currently assigned");
            }

            return Ok("Asset returned successfully");
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("history/{assetId}")]
        public IActionResult GetAssetAllocationHistory(int assetId,int page=1,int pageSize=5) 
        {
            return Ok(
        _assetService.GetAllocationHistory(
            assetId,
            page,
            pageSize));
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("available")]
        public IActionResult GetAvailableAssets(
    int page = 1,
    int pageSize = 5)
        {
            return Ok(
                _assetService.GetAvailableAssets(page, pageSize)
            );
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("myassets")]
        public IActionResult GetMyAssets()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return Unauthorized();
            }

            return Ok(_assetService.GetMyAssets(email));
        }

    }
}