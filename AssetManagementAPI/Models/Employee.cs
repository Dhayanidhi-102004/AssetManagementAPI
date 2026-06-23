using System.ComponentModel.DataAnnotations;

namespace AssetManagementAPI.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";

        public string Gender { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}