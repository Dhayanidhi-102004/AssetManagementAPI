
using Microsoft.EntityFrameworkCore;
using AssetManagementAPI.Models;
namespace AssetManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetRequest> AssetRequests { get; set; }
        public DbSet<AssetAllocation> AssetAllocations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}
