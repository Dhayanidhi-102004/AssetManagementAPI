namespace AssetManagementAPI.Models
{
    public class AssetAllocation
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public string Status { get; set; } = "Active";
    }
}
