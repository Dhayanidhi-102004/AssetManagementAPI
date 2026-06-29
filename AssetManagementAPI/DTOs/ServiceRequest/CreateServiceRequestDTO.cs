using AssetManagementAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagementAPI.DTOs.ServiceRequest
{
    public class CreateServiceRequestDTO
    {
        [Required]
        public int AssetId { get; set; }

        [Required]
        public IssueType IssueType { get; set; } 

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}

